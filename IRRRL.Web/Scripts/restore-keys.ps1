# ============================================================================
# Encryption Keys Restore Script
# ============================================================================
# This script restores encryption keys from a password-protected backup
# 
# USAGE: 
#   .\restore-keys.ps1
#   .\restore-keys.ps1 -BackupFile "path\to\backup.zip"
#
# WHAT IT DOES:
#   1. Finds the most recent backup (or uses specified file)
#   2. Asks for the backup password
#   3. Extracts keys to the Keys folder
#   4. Validates the restoration
#
# WHEN TO USE:
#   - Setting up on a new computer
#   - After accidentally deleting Keys folder
#   - Sharing keys with a teammate
#   - Recovering from a system failure
#
# IMPORTANT:
#   - You must know the backup password
#   - This will OVERWRITE existing keys (if any)
#   - Test decryption after restore to verify it worked
# ============================================================================

param(
    [string]$BackupFile,
    [string]$KeysPath = "../Keys",
    [string]$BackupPath = "../KeyBackups",
    [switch]$Help
)

# Show help if requested
if ($Help) {
    Write-Host @"
Encryption Keys Restore Script
===============================

USAGE:
    .\restore-keys.ps1 [-BackupFile <file>] [-KeysPath <path>] [-Help]

OPTIONS:
    -BackupFile  Specific backup file to restore (optional)
    -KeysPath    Where to restore keys (default: ../Keys)
    -BackupPath  Where backups are stored (default: ../KeyBackups)
    -Help        Show this help message

EXAMPLES:
    .\restore-keys.ps1
    .\restore-keys.ps1 -BackupFile "KeyBackups\EncryptionKeys_20231115_143022.zip"

NOTES:
    - If no BackupFile specified, will use most recent backup
    - You will be prompted for the backup password
    - Existing keys will be backed up before restoration
"@
    exit 0
}

# ============================================================================
# FUNCTIONS
# ============================================================================

function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

function Test-7ZipAvailable {
    $7zipPaths = @(
        "C:\Program Files\7-Zip\7z.exe",
        "C:\Program Files (x86)\7-Zip\7z.exe",
        "$env:ProgramFiles\7-Zip\7z.exe",
        "$env:ProgramFiles(x86)\7-Zip\7z.exe"
    )
    
    foreach ($path in $7zipPaths) {
        if (Test-Path $path) {
            return $path
        }
    }
    return $null
}

# ============================================================================
# MAIN SCRIPT
# ============================================================================

Write-ColorOutput "========================================" "Cyan"
Write-ColorOutput "  Encryption Keys Restore Tool" "Cyan"
Write-ColorOutput "========================================" "Cyan"
Write-Host ""

# Step 1: Find backup file
if (-not $BackupFile) {
    # Look for most recent backup
    $backupFullPath = Join-Path $PSScriptRoot $BackupPath
    
    if (-not (Test-Path $backupFullPath)) {
        Write-ColorOutput "ERROR: Backup directory not found: $backupFullPath" "Red"
        Write-Host ""
        Write-Host "No backups available. Make sure:"
        Write-Host "  1. You have created a backup using backup-keys.ps1"
        Write-Host "  2. The backup path is correct"
        Write-Host ""
        Write-Host "Run with -Help for more options"
        exit 1
    }
    
    $backups = Get-ChildItem -Path $backupFullPath -Filter "EncryptionKeys_*.zip" | Sort-Object LastWriteTime -Descending
    
    if ($backups.Count -eq 0) {
        Write-ColorOutput "ERROR: No backup files found in: $backupFullPath" "Red"
        Write-Host ""
        Write-Host "Create a backup first using: .\backup-keys.ps1"
        exit 1
    }
    
    if ($backups.Count -gt 1) {
        Write-ColorOutput "Found $($backups.Count) backups:" "Yellow"
        Write-Host ""
        for ($i = 0; $i -lt [Math]::Min(5, $backups.Count); $i++) {
            $backup = $backups[$i]
            $age = (Get-Date) - $backup.LastWriteTime
            $ageStr = if ($age.Days -gt 0) { "$($age.Days) days ago" } 
                      elseif ($age.Hours -gt 0) { "$($age.Hours) hours ago" }
                      else { "$($age.Minutes) minutes ago" }
            Write-Host "  [$($i+1)] $($backup.Name) ($ageStr)"
        }
        Write-Host ""
        
        $selection = Read-Host "Select backup number (1-$([Math]::Min(5, $backups.Count))) or press Enter for most recent"
        
        if ($selection) {
            $index = [int]$selection - 1
            if ($index -lt 0 -or $index -ge $backups.Count) {
                Write-ColorOutput "ERROR: Invalid selection" "Red"
                exit 1
            }
            $BackupFile = $backups[$index].FullName
        } else {
            $BackupFile = $backups[0].FullName
        }
    } else {
        $BackupFile = $backups[0].FullName
    }
}

# Validate backup file exists
if (-not (Test-Path $BackupFile)) {
    Write-ColorOutput "ERROR: Backup file not found: $BackupFile" "Red"
    exit 1
}

Write-ColorOutput "✓ Using backup: $(Split-Path -Leaf $BackupFile)" "Green"
$backupInfo = Get-Item $BackupFile
Write-Host "  Created: $($backupInfo.LastWriteTime)"
Write-Host "  Size: $([math]::Round($backupInfo.Length / 1KB, 2)) KB"
Write-Host ""

# Step 2: Check if Keys folder exists and backup if needed
$keysFullPath = Join-Path $PSScriptRoot $KeysPath

if (Test-Path $keysFullPath) {
    $existingKeys = Get-ChildItem -Path $keysFullPath -File
    
    if ($existingKeys.Count -gt 0) {
        Write-ColorOutput "WARNING: Keys folder already contains $($existingKeys.Count) file(s)" "Yellow"
        Write-Host ""
        $overwrite = Read-Host "Overwrite existing keys? (yes/no)"
        
        if ($overwrite -ne "yes") {
            Write-Host "Restore cancelled by user."
            exit 0
        }
        
        # Backup existing keys before overwriting
        $tempBackup = Join-Path $keysFullPath "../Keys_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
        Write-Host ""
        Write-Host "Backing up existing keys to: $tempBackup"
        Copy-Item -Path $keysFullPath -Destination $tempBackup -Recurse -Force
        Write-ColorOutput "✓ Existing keys backed up" "Green"
        Write-Host ""
        
        # Clear existing keys
        Remove-Item -Path "$keysFullPath\*" -Force
    }
} else {
    # Create Keys directory
    New-Item -ItemType Directory -Path $keysFullPath -Force | Out-Null
    Write-ColorOutput "✓ Created Keys directory" "Green"
    Write-Host ""
}

# Step 3: Prompt for password
Write-ColorOutput "Enter the backup password:" "Yellow"
$password = Read-Host "Password" -AsSecureString

# Convert SecureString to plain text
$pwd = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))

Write-Host ""
Write-ColorOutput "Extracting keys..." "Cyan"
Write-Host ""

# Step 4: Extract the backup
$7zipPath = Test-7ZipAvailable

if ($7zipPath) {
    Write-Host "Using 7-Zip to extract..."
    
    $args = @(
        "x",                        # Extract
        "-p$pwd",                   # Password
        $BackupFile,                # Source file
        "-o$keysFullPath",          # Output directory
        "-y"                        # Yes to all prompts
    )
    
    & $7zipPath $args 2>&1 | Out-Null
    
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "ERROR: Failed to extract backup!" "Red"
        Write-Host ""
        Write-Host "Possible reasons:"
        Write-Host "  - Incorrect password"
        Write-Host "  - Corrupted backup file"
        Write-Host "  - Insufficient permissions"
        exit 1
    }
    
} else {
    Write-Host "7-Zip not found, using PowerShell extraction..."
    
    try {
        # PowerShell's Expand-Archive doesn't support password-protected ZIPs natively
        # This is a limitation for backups created without 7-Zip
        Expand-Archive -Path $BackupFile -DestinationPath $keysFullPath -Force
        
    } catch {
        Write-ColorOutput "ERROR: Failed to extract backup: $_" "Red"
        Write-Host ""
        Write-Host "If this backup was created with 7-Zip, you need 7-Zip to restore it."
        Write-Host "Download from: https://www.7-zip.org/"
        exit 1
    }
}

# Clear password from memory
$pwd = $null

# Step 5: Validate restoration
$restoredKeys = Get-ChildItem -Path $keysFullPath -File

if ($restoredKeys.Count -eq 0) {
    Write-ColorOutput "ERROR: No keys were extracted!" "Red"
    Write-Host ""
    Write-Host "The backup may be:"
    Write-Host "  - Empty"
    Write-Host "  - Corrupted"
    Write-Host "  - Password-protected (wrong password)"
    exit 1
}

Write-ColorOutput "✓ Keys restored successfully!" "Green"
Write-Host "  Restored $($restoredKeys.Count) file(s)"
Write-Host ""

# Step 6: Show results
Write-ColorOutput "========================================" "Cyan"
Write-ColorOutput "  Restore Complete!" "Green"
Write-ColorOutput "========================================" "Cyan"
Write-Host ""
Write-Host "Keys Location: $keysFullPath"
Write-Host "Files Restored: $($restoredKeys.Count)"
Write-Host ""

# Step 7: Provide next steps
Write-ColorOutput "NEXT STEPS:" "Yellow"
Write-Host ""
Write-Host "1. TEST THE RESTORATION:"
Write-Host "   - Start the application"
Write-Host "   - Try to view existing SSN data"
Write-Host "   - If you can decrypt SSNs, restoration succeeded!"
Write-Host ""
Write-Host "2. IF DECRYPTION FAILS:"
Write-Host "   - Wrong backup file (keys don't match database)"
Write-Host "   - Backup was corrupted"
Write-Host "   - Try a different backup"
Write-Host ""
Write-Host "3. SECURE YOUR KEYS:"
Write-Host "   - Keys folder should NOT be committed to git"
Write-Host "   - Keep backup files secure"
Write-Host "   - Consider Azure Key Vault for production"
Write-Host ""


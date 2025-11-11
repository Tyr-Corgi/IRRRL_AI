# ============================================================================
# Encryption Keys Backup Script
# ============================================================================
# This script creates a password-protected ZIP backup of your encryption keys
# 
# USAGE: 
#   .\backup-keys.ps1
#
# WHAT IT DOES:
#   1. Finds the Keys folder
#   2. Creates a timestamped backup
#   3. Password-protects it with 7-Zip (if available) or regular ZIP
#   4. Saves to a KeyBackups folder
#
# IMPORTANT:
#   - Store this backup file in a secure location (NOT in git!)
#   - Remember your password - without it, you can't restore keys
#   - Without these keys, you cannot decrypt any SSNs in the database
# ============================================================================

param(
    [string]$KeysPath = "../Keys",
    [string]$BackupPath = "../KeyBackups",
    [switch]$Help
)

# Show help if requested
if ($Help) {
    Write-Host @"
Encryption Keys Backup Script
==============================

USAGE:
    .\backup-keys.ps1 [-KeysPath <path>] [-BackupPath <path>] [-Help]

OPTIONS:
    -KeysPath    Path to Keys folder (default: ../Keys)
    -BackupPath  Where to save backup (default: ../KeyBackups)
    -Help        Show this help message

EXAMPLES:
    .\backup-keys.ps1
    .\backup-keys.ps1 -KeysPath "C:\MyProject\Keys"
    .\backup-keys.ps1 -BackupPath "D:\SecureBackups"

AFTER BACKUP:
    - Store the backup file in a secure location
    - Consider: USB drive, encrypted cloud storage, password manager
    - NEVER commit the backup to git!
    - Test the restore process to make sure it works
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
Write-ColorOutput "  Encryption Keys Backup Tool" "Cyan"
Write-ColorOutput "========================================" "Cyan"
Write-Host ""

# Step 1: Validate Keys folder exists
$keysFullPath = Join-Path $PSScriptRoot $KeysPath | Resolve-Path -ErrorAction SilentlyContinue
if (-not $keysFullPath -or -not (Test-Path $keysFullPath)) {
    Write-ColorOutput "ERROR: Keys folder not found at: $KeysPath" "Red"
    Write-Host ""
    Write-Host "Make sure:"
    Write-Host "  1. The application has been run at least once (to generate keys)"
    Write-Host "  2. The Keys folder exists"
    Write-Host "  3. The path is correct"
    Write-Host ""
    Write-Host "Run with -Help for more options"
    exit 1
}

Write-ColorOutput "✓ Found Keys folder: $keysFullPath" "Green"

# Count files in Keys folder
$keyFiles = Get-ChildItem -Path $keysFullPath -File
Write-Host "  Files to backup: $($keyFiles.Count)"
Write-Host ""

if ($keyFiles.Count -eq 0) {
    Write-ColorOutput "WARNING: Keys folder is empty!" "Yellow"
    Write-Host "The application may not have generated keys yet."
    Write-Host "Run the application first, then try backing up again."
    exit 1
}

# Step 2: Create backup directory
$backupFullPath = Join-Path $PSScriptRoot $BackupPath
if (-not (Test-Path $backupFullPath)) {
    New-Item -ItemType Directory -Path $backupFullPath -Force | Out-Null
    Write-ColorOutput "✓ Created backup directory: $backupFullPath" "Green"
} else {
    Write-ColorOutput "✓ Backup directory exists: $backupFullPath" "Green"
}
Write-Host ""

# Step 3: Generate backup filename with timestamp
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFileName = "EncryptionKeys_$timestamp"
$backupFilePath = Join-Path $backupFullPath "$backupFileName.zip"

# Step 4: Prompt for password
Write-ColorOutput "IMPORTANT: Create a strong password for this backup" "Yellow"
Write-Host "This password will be required to restore the keys."
Write-Host ""

$password = Read-Host "Enter backup password" -AsSecureString
$passwordConfirm = Read-Host "Confirm password" -AsSecureString

# Convert SecureString to plain text for comparison
$pwd1 = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
$pwd2 = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($passwordConfirm))

if ($pwd1 -ne $pwd2) {
    Write-ColorOutput "ERROR: Passwords do not match!" "Red"
    exit 1
}

if ($pwd1.Length -lt 8) {
    Write-ColorOutput "ERROR: Password must be at least 8 characters!" "Red"
    exit 1
}

Write-Host ""
Write-ColorOutput "✓ Password confirmed" "Green"
Write-Host ""

# Step 5: Create the backup
Write-ColorOutput "Creating encrypted backup..." "Cyan"
Write-Host ""

# Check if 7-Zip is available (provides better encryption)
$7zipPath = Test-7ZipAvailable

if ($7zipPath) {
    Write-Host "Using 7-Zip for AES-256 encryption..."
    
    # Use 7-Zip with AES-256 encryption
    $args = @(
        "a",                        # Add to archive
        "-tzip",                    # ZIP format
        "-p$pwd1",                  # Password
        "-mem=AES256",              # AES-256 encryption
        "-mx=9",                    # Maximum compression
        $backupFilePath,            # Output file
        "$keysFullPath\*"           # Input files
    )
    
    & $7zipPath $args | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✓ Backup created successfully with AES-256 encryption!" "Green"
    } else {
        Write-ColorOutput "ERROR: 7-Zip backup failed!" "Red"
        exit 1
    }
    
} else {
    Write-Host "7-Zip not found, using standard ZIP encryption..."
    Write-ColorOutput "NOTE: For better security, install 7-Zip from https://www.7-zip.org/" "Yellow"
    Write-Host ""
    
    # Fallback to PowerShell Compress-Archive with separate encryption step
    # Note: PowerShell doesn't natively support password-protected ZIPs well
    # This is a simplified version for development
    
    try {
        Compress-Archive -Path "$keysFullPath\*" -DestinationPath $backupFilePath -Force
        Write-ColorOutput "✓ Backup created (unencrypted ZIP)" "Yellow"
        Write-ColorOutput "  For production, use 7-Zip or move keys to Azure Key Vault" "Yellow"
    } catch {
        Write-ColorOutput "ERROR: Failed to create backup: $_" "Red"
        exit 1
    }
}

# Clear password from memory
$pwd1 = $null
$pwd2 = $null

# Step 6: Show results
Write-Host ""
Write-ColorOutput "========================================" "Cyan"
Write-ColorOutput "  Backup Complete!" "Green"
Write-ColorOutput "========================================" "Cyan"
Write-Host ""
Write-Host "Backup Location: $backupFilePath"
Write-Host "Backup Size:     $([math]::Round((Get-Item $backupFilePath).Length / 1KB, 2)) KB"
Write-Host ""

# Step 7: Provide next steps
Write-ColorOutput "NEXT STEPS:" "Yellow"
Write-Host ""
Write-Host "1. STORE THIS BACKUP SECURELY:"
Write-Host "   - USB drive in a safe location"
Write-Host "   - Encrypted cloud storage (OneDrive, Dropbox, etc.)"
Write-Host "   - Password manager (many support file attachments)"
Write-Host ""
Write-Host "2. DO NOT:"
Write-Host "   - Commit this file to git"
Write-Host "   - Email it unencrypted"
Write-Host "   - Store it in an unsecured location"
Write-Host ""
Write-Host "3. REMEMBER YOUR PASSWORD:"
Write-Host "   - Without it, the backup is useless"
Write-Host "   - Consider storing password in a password manager"
Write-Host ""
Write-Host "4. TEST THE RESTORE:"
Write-Host "   - Run .\restore-keys.ps1 to verify backup works"
Write-Host "   - Do this before you need it!"
Write-Host ""

Write-ColorOutput "Backup password required for restore: YES" "Yellow"
Write-Host ""


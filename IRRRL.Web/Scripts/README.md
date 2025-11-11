# Encryption Key Management Scripts

These scripts help you backup and restore encryption keys for the IRRRL application.

---

## 🎯 Why You Need This

**The Problem:**
- Encryption keys are stored in `./Keys` folder
- They're NOT in git (for security)
- Without keys, you can't decrypt SSNs in the database
- If you lose keys = you lose all encrypted data permanently

**The Solution:**
These scripts create secure, password-protected backups of your keys.

---

## 📦 What's Included

### 1. `backup-keys.ps1`
Creates a password-protected backup of your encryption keys.

**When to use:**
- Before major changes to your system
- Before reinstalling Windows
- When adding a teammate (they'll need a copy)
- Regular backups (weekly/monthly)

### 2. `restore-keys.ps1`
Restores keys from a backup.

**When to use:**
- Setting up on a new computer
- After accidentally deleting Keys folder
- When teammate needs the keys
- Recovering from system failure

---

## 🚀 How to Use

### Creating a Backup

1. Open PowerShell
2. Navigate to the Scripts folder:
   ```powershell
   cd "C:\Mac\Home\Desktop\Repos\IRRRL AI\IRRRL.Web\Scripts"
   ```

3. Run the backup script:
   ```powershell
   .\backup-keys.ps1
   ```

4. Create a strong password when prompted
5. Find your backup in `../KeyBackups/` folder

**Example:**
```
PS> .\backup-keys.ps1

========================================
  Encryption Keys Backup Tool
========================================

✓ Found Keys folder: C:\...\Keys
  Files to backup: 3

✓ Backup directory exists: C:\...\KeyBackups

IMPORTANT: Create a strong password for this backup
This password will be required to restore the keys.

Enter backup password: ********
Confirm password: ********

✓ Password confirmed

Creating encrypted backup...

Using 7-Zip for AES-256 encryption...
✓ Backup created successfully with AES-256 encryption!

========================================
  Backup Complete!
========================================

Backup Location: C:\...\KeyBackups\EncryptionKeys_20231115_143022.zip
Backup Size:     12.45 KB
```

---

### Restoring from a Backup

1. Open PowerShell
2. Navigate to the Scripts folder:
   ```powershell
   cd "C:\Mac\Home\Desktop\Repos\IRRRL AI\IRRRL.Web\Scripts"
   ```

3. Run the restore script:
   ```powershell
   .\restore-keys.ps1
   ```

4. Select which backup to restore (or press Enter for most recent)
5. Enter the backup password
6. Keys will be extracted to `../Keys/` folder

**Example:**
```
PS> .\restore-keys.ps1

========================================
  Encryption Keys Restore Tool
========================================

Found 3 backups:

  [1] EncryptionKeys_20231115_143022.zip (2 hours ago)
  [2] EncryptionKeys_20231114_091530.zip (1 day ago)
  [3] EncryptionKeys_20231110_163045.zip (5 days ago)

Select backup number (1-3) or press Enter for most recent: 

✓ Using backup: EncryptionKeys_20231115_143022.zip
  Created: 11/15/2023 2:30:22 PM
  Size: 12.45 KB

Enter the backup password:
Password: ********

Extracting keys...

Using 7-Zip to extract...
✓ Keys restored successfully!
  Restored 3 file(s)
```

---

## 🔐 Security Best Practices

### ✅ DO:
1. **Use a strong password** (at least 12 characters, mix of letters/numbers/symbols)
2. **Store backups securely:**
   - Encrypted USB drive
   - Password manager (many support file attachments)
   - Encrypted cloud storage (OneDrive Personal Vault, Dropbox, etc.)
3. **Remember your password** (store in password manager)
4. **Test restores regularly** (make sure backups work!)
5. **Keep multiple backups** (different locations)
6. **Backup before major changes**

### ❌ DON'T:
1. **Never commit backups to git**
2. **Never email backups unencrypted**
3. **Never use a weak password** (no "password123")
4. **Never store backup in the same location as original keys**
5. **Never lose all copies** (keep at least 2 backups)

---

## 🛠️ Requirements

### Windows PowerShell
- Included with Windows (already installed)
- Run scripts from PowerShell, not Command Prompt

### 7-Zip (Recommended but Optional)
- **With 7-Zip:** Backups use AES-256 encryption (military-grade)
- **Without 7-Zip:** Backups use standard ZIP (less secure)

**Download 7-Zip:** https://www.7-zip.org/

To check if you have 7-Zip:
```powershell
Test-Path "C:\Program Files\7-Zip\7z.exe"
```
If it returns `True`, you're good!

---

## 📋 Common Scenarios

### Scenario 1: New Teammate Joining
```powershell
# On your computer (create backup)
.\backup-keys.ps1

# Share the backup file securely (not via email!)
# - Password manager
# - Encrypted USB
# - Secure file share

# On teammate's computer (restore keys)
.\restore-keys.ps1 -BackupFile "path\to\backup.zip"
```

### Scenario 2: Moving to New Computer
```powershell
# OLD COMPUTER - Create backup
.\backup-keys.ps1

# Copy backup to USB drive or cloud storage

# NEW COMPUTER - Restore keys
.\restore-keys.ps1
```

### Scenario 3: Accidentally Deleted Keys
```powershell
# Don't panic! Restore from backup
.\restore-keys.ps1

# Select most recent backup
# Enter password
# Keys restored!
```

### Scenario 4: Regular Backup Schedule
```powershell
# Every Friday (or weekly)
.\backup-keys.ps1

# Rotate backups (keep last 4 weeks)
# Delete backups older than 1 month
```

---

## 🆘 Troubleshooting

### "Keys folder not found"
**Problem:** The application hasn't generated keys yet.
**Solution:** 
1. Run the application at least once
2. The Keys folder will be created automatically
3. Then run backup script

### "Password incorrect" during restore
**Problem:** Wrong password or corrupted backup.
**Solution:**
1. Try again with correct password
2. Try a different backup file
3. Check if backup file is corrupted

### "7-Zip not found"
**Problem:** 7-Zip not installed or not in default location.
**Solution:**
- Install 7-Zip from https://www.7-zip.org/
- Or use without 7-Zip (less secure but works)

### "Execution Policy" error
**Problem:** PowerShell script execution blocked.
**Solution:**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Keys restored but can't decrypt data
**Problem:** Wrong backup file (keys don't match database).
**Solution:**
1. Make sure you're using the correct backup
2. Check if database and keys are from same environment
3. Try earlier backup

---

## 🚀 Advanced Usage

### Specify Custom Paths
```powershell
# Backup keys from custom location
.\backup-keys.ps1 -KeysPath "D:\MyKeys" -BackupPath "E:\Backups"

# Restore to custom location
.\restore-keys.ps1 -KeysPath "D:\MyKeys"
```

### Restore Specific Backup
```powershell
.\restore-keys.ps1 -BackupFile "KeyBackups\EncryptionKeys_20231110_163045.zip"
```

### Get Help
```powershell
.\backup-keys.ps1 -Help
.\restore-keys.ps1 -Help
```

---

## 📊 Backup File Format

**Filename:** `EncryptionKeys_YYYYMMDD_HHMMSS.zip`

**Example:** `EncryptionKeys_20231115_143022.zip`
- `20231115` = November 15, 2023
- `143022` = 2:30:22 PM

**Contents:** All files from `Keys/` folder

**Encryption:** 
- With 7-Zip: AES-256 (very secure)
- Without 7-Zip: Standard ZIP password (less secure)

**Size:** Typically 10-50 KB

---

## 🎓 Understanding the Keys

### What's in the Keys folder?
```
Keys/
├── key-{guid}.xml          (Primary encryption key)
├── key-{guid}.xml          (Previous key - for key rotation)
└── ...
```

### What do these files do?
- Contain the actual encryption keys
- Used by ASP.NET Data Protection API
- Automatically managed by the framework
- Include key ID, creation date, expiration date, algorithm info

### Can I look inside?
Yes, but you shouldn't modify them! They're XML files with base64-encoded keys.

**Example structure:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<key id="12345678-90ab-cdef-1234-567890abcdef" version="1">
  <creationDate>2023-11-15T14:30:22Z</creationDate>
  <activationDate>2023-11-15T14:30:22Z</activationDate>
  <expirationDate>2024-02-13T14:30:22Z</expirationDate>
  <descriptor>
    <encryption algorithm="AES_256_CBC" />
    <validation algorithm="HMACSHA256" />
    <masterKey>...</masterKey>
  </descriptor>
</key>
```

---

## 🔄 Next Steps: Production Deployment

These scripts are great for development, but for production you should use:

### **Azure Key Vault** (Recommended)
- Professional key management
- Access from any computer
- No manual backups needed
- Automatic key rotation
- Audit logs

### **Setup Guide:** See `SECURITY_IMPLEMENTATION.md` for Azure Key Vault setup

---

## ❓ Questions?

**Q: How often should I backup?**
A: At least weekly, and always before major changes.

**Q: How many backups should I keep?**
A: Keep at least 3-4 recent backups. Rotate old ones out.

**Q: Where should I store backups?**
A: 
1. USB drive (encrypted)
2. Password manager (many support file attachments)
3. Encrypted cloud storage (OneDrive Personal Vault, etc.)
4. NOT in git, NOT in email

**Q: What if I forget the password?**
A: The backup is unrecoverable. Always store passwords in a password manager!

**Q: Can I use the same keys on multiple environments?**
A: Yes for dev/staging, but production should have separate keys.

**Q: What if someone gets my backup file?**
A: They still need the password. Use a strong password (12+ characters).

---

## 📚 Related Documentation

- `SECURITY_IMPLEMENTATION.md` - Full encryption documentation
- `.gitignore` - Keys are excluded from git
- `Program.cs` - Key storage configuration

---

**Remember: Your encryption keys are like the master password to all encrypted data. Protect them!** 🔐


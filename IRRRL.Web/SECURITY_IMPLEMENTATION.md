# SSN Encryption Implementation Guide

## Overview
This document explains how Social Security Numbers (SSNs) are encrypted in the IRRRL application using ASP.NET Core Data Protection API.

---

## How It Works

### 1. **The Problem**
- SSNs are sensitive personal information (PII)
- Storing them as plaintext ("123-45-6789") in the database is a **security risk**
- If database is compromised, SSNs are exposed

### 2. **The Solution: Encryption**
We encrypt SSNs **before** storing them in the database:

```
User enters: "123-45-6789"
         ↓
App encrypts: "CfDJ8Kx7L9pQ..." (200+ character encrypted string)
         ↓
Stored in DB: "CfDJ8Kx7L9pQ..."
         ↓
When needed: Decrypt back to "123-45-6789" for display
```

---

## The Technology: ASP.NET Core Data Protection API

### What is it?
Microsoft's built-in encryption system that:
- **Automatically manages encryption keys**
- Uses **AES-256** encryption (industry standard, very secure)
- **Rotates keys** periodically for enhanced security
- **Prevents tampering** with authentication tags (HMAC)

### Where are the keys stored?
- **Development**: `./Keys` folder (in your project directory)
- **Production**: Should use Azure Key Vault or similar secure storage

**IMPORTANT**: The `./Keys` folder should be:
- Added to `.gitignore` (never commit encryption keys to git!)
- Backed up securely
- Protected with file system permissions

---

## Implementation Details

### Files Created

#### 1. `IRRRL.Core/Interfaces/IDataProtectionService.cs`
**What it does**: Defines the contract for encryption/decryption services.

```csharp
public interface IDataProtectionService
{
    string Encrypt(string plaintext);      // Encrypts any string
    string Decrypt(string ciphertext);     // Decrypts any string
    string? EncryptSSN(string? ssn);       // Encrypts SSN specifically
    string? DecryptSSN(string? encryptedSsn); // Decrypts SSN
}
```

#### 2. `IRRRL.Infrastructure/Services/DataProtectionService.cs`
**What it does**: Implements the actual encryption logic.

**Key Concepts**:
- **Purpose String** (`"IRRRL.SSN.Protection.v1"`): 
  - Acts as a "namespace" for encryption
  - Data encrypted with one purpose can't be decrypted by another
  - Provides additional security layer

- **Protect/Unprotect Methods**:
  - `Protect()`: Takes plaintext → Returns encrypted base64 string
  - `Unprotect()`: Takes encrypted string → Returns plaintext
  - Automatically handles IV (initialization vector) and authentication

**Example**:
```csharp
// Encrypting
var encrypted = _protector.Protect("123456789");
// Result: "CfDJ8Kx7L9pQ3R5S..." (long encrypted string)

// Decrypting
var decrypted = _protector.Unprotect("CfDJ8Kx7L9pQ3R5S...");
// Result: "123456789"
```

#### 3. `IRRRL.Web/Program.cs` (Updated)
**What we added**:
```csharp
// Configure Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"./Keys"))
    .SetApplicationName("IRRRL");

// Register our encryption service
builder.Services.AddScoped<IDataProtectionService, DataProtectionService>();
```

**Explanation**:
- `PersistKeysToFileSystem`: Tells ASP.NET where to store encryption keys
- `SetApplicationName`: Isolates keys to this specific application
- `AddScoped`: Creates one instance per request (efficient and safe)

#### 4. Database Changes
**Updated**: `ApplicationDbContext.cs`
```csharp
// OLD: entity.Property(e => e.SSN).HasMaxLength(11);
// NEW: entity.Property(e => e.SSN).HasMaxLength(500);
```

**Why 500 characters?**
- Original SSN: "123-45-6789" = 11 characters
- Encrypted SSN: ~200-300 characters (varies due to random IV)
- 500 gives us room for different encryption algorithms in the future

#### 5. Migration File
**Created**: `UpdateSSNFieldForEncryption.cs`

**What it does**: Updates the database schema to expand the SSN column.

---

## How to Use in Code

### When Saving an SSN:

```csharp
// In your Blazor component or controller
@inject IDataProtectionService DataProtection

// User enters SSN in form
var userEnteredSSN = "123-45-6789";

// Encrypt before saving to database
var borrower = new Borrower
{
    FirstName = "John",
    LastName = "Doe",
    SSN = DataProtection.EncryptSSN(userEnteredSSN), // Encrypted!
    // ... other properties
};

await _context.Borrowers.AddAsync(borrower);
await _context.SaveChangesAsync();
```

**What's stored in database**: `"CfDJ8Kx7L9pQ3R5S7T9V..."`

### When Displaying an SSN:

```csharp
// Read from database
var borrower = await _context.Borrowers.FindAsync(id);

// Decrypt for display
var decryptedSSN = DataProtection.DecryptSSN(borrower.SSN);
// Result: "123-45-6789"

// For security, only show last 4 digits
var masked = $"***-**-{decryptedSSN.Substring(decryptedSSN.Length - 4)}";
// Result: "***-**-6789"
```

---

## Security Best Practices

### ✅ DO:
1. **Always encrypt SSN before saving** to database
2. **Mask SSN in UI** (show `***-**-6789` instead of full SSN)
3. **Log SSN access** in audit logs (who viewed, when)
4. **Backup encryption keys** securely
5. **Use HTTPS** in production (encrypts data in transit)
6. **Limit SSN access** to authorized users only

### ❌ DON'T:
1. **Never log plaintext SSNs** (logs are often not encrypted)
2. **Never commit encryption keys** to git
3. **Never display full SSN** unless absolutely necessary
4. **Never send SSN in URLs** or query parameters
5. **Never store SSN in cookies** or browser storage

---

## What Happens if Keys are Lost?

**Critical**: If you lose the encryption keys:
- **All encrypted SSNs become unrecoverable**
- You cannot decrypt existing data
- You'll need to collect SSNs again from users

**Prevention**:
- **Backup the `./Keys` folder** regularly
- In production, use **Azure Key Vault** or **AWS KMS**
- Have a **disaster recovery plan**

---

## Compliance Notes

This implementation helps with:
- **GLBA** (Gramm-Leach-Bliley Act): Requires financial institutions to protect customer data
- **FCRA** (Fair Credit Reporting Act): Regulates consumer credit information
- **VA Requirements**: Veterans Affairs has strict PII protection requirements

**What's still needed for full compliance**:
1. **Encryption at rest** (database-level encryption)
2. **Encryption in transit** (HTTPS/TLS)
3. **Access controls** (role-based permissions)
4. **Audit logging** (track all SSN access)
5. **Data retention policies** (when to delete SSNs)
6. **Incident response plan** (what to do if breached)

---

## Testing the Encryption

### Simple Test:
```csharp
[Fact]
public void SSN_Encryption_Works()
{
    // Arrange
    var service = new DataProtectionService(provider);
    var originalSSN = "123-45-6789";
    
    // Act
    var encrypted = service.EncryptSSN(originalSSN);
    var decrypted = service.DecryptSSN(encrypted);
    
    // Assert
    Assert.NotEqual(originalSSN, encrypted); // Should be different
    Assert.Equal("123-45-6789", decrypted);  // Should match original
    Assert.True(encrypted.Length > 100);      // Should be long
}
```

---

## Next Steps

### Immediate (To-Do):
1. **Apply the migration**: Update the database schema
2. **Add .gitignore entry**: Prevent committing encryption keys
3. **Update Blazor forms**: Use encryption service when submitting SSN
4. **Test thoroughly**: Verify encryption/decryption works

### Future Enhancements:
1. **Move to Azure Key Vault** (production-ready key management)
2. **Add audit logging** (track SSN access)
3. **Implement key rotation** (change keys periodically)
4. **Add data masking** (always show `***-**-6789` in UI)
5. **Encrypt other PII** (date of birth, VA file number, etc.)

---

## Questions & Troubleshooting

### Q: Can I search by SSN in the database?
**A**: No, encrypted data cannot be searched directly. Options:
- Decrypt and compare in application code
- Use a hashed version for lookups (separate column)
- Use searchable encryption (advanced, requires special libraries)

### Q: What if I need to change the encryption algorithm?
**A**: The purpose string includes version (`v1`). To upgrade:
1. Create new service with `v2` purpose string
2. Decrypt with `v1`, re-encrypt with `v2`
3. Run data migration script

### Q: Is this HIPAA compliant?
**A**: This is a good start, but HIPAA requires:
- Business Associate Agreements (BAAs)
- Physical security measures
- Regular security audits
- Consult a compliance expert for full HIPAA compliance

---

## References
- [ASP.NET Core Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/)
- [NIST Encryption Standards](https://csrc.nist.gov/projects/cryptographic-standards-and-guidelines)
- [VA Privacy Requirements](https://www.oprm.va.gov/privacy/)


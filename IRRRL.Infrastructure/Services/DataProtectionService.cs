using IRRRL.Core.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace IRRRL.Infrastructure.Services;

/// <summary>
/// Implements encryption/decryption using ASP.NET Core Data Protection API
/// This service encrypts sensitive data like SSN before storing in database
/// </summary>
public class DataProtectionService : IDataProtectionService
{
    private readonly IDataProtector _protector;

    /// <summary>
    /// Constructor receives IDataProtectionProvider from dependency injection
    /// We create a "protector" with a specific purpose string - this ensures
    /// data encrypted for "SSN" purposes can't be decrypted by other protectors
    /// </summary>
    public DataProtectionService(IDataProtectionProvider provider)
    {
        // Create a protector specifically for SSN data
        // The purpose string acts as additional security - even if keys are compromised,
        // different purposes can't decrypt each other's data
        _protector = provider.CreateProtector("IRRRL.SSN.Protection.v1");
    }

    /// <summary>
    /// Encrypts a plaintext string
    /// Example: "123-45-6789" becomes "CfDJ8Kx..." (long encrypted string)
    /// </summary>
    public string Encrypt(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
        {
            throw new ArgumentException("Cannot encrypt null or empty string", nameof(plaintext));
        }

        // The Protect method:
        // 1. Takes your plaintext
        // 2. Generates a unique initialization vector (IV) for this encryption
        // 3. Encrypts using AES-256
        // 4. Adds authentication tag (HMAC) to prevent tampering
        // 5. Returns base64-encoded result
        return _protector.Protect(plaintext);
    }

    /// <summary>
    /// Decrypts an encrypted string back to plaintext
    /// </summary>
    public string Decrypt(string ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext))
        {
            throw new ArgumentException("Cannot decrypt null or empty string", nameof(ciphertext));
        }

        try
        {
            // The Unprotect method:
            // 1. Decodes the base64 string
            // 2. Verifies the authentication tag (ensures data wasn't tampered with)
            // 3. Decrypts using AES-256
            // 4. Returns original plaintext
            return _protector.Unprotect(ciphertext);
        }
        catch (Exception ex)
        {
            // If decryption fails (wrong key, corrupted data, tampered data), throw clear error
            throw new InvalidOperationException("Failed to decrypt data. The data may be corrupted or encrypted with a different key.", ex);
        }
    }

    /// <summary>
    /// Encrypts an SSN, handling null/empty values
    /// </summary>
    public string? EncryptSSN(string? ssn)
    {
        // If no SSN provided, return null (don't encrypt empty values)
        if (string.IsNullOrWhiteSpace(ssn))
        {
            return null;
        }

        // Clean the SSN (remove dashes, spaces) before encrypting
        // Store in consistent format
        var cleanedSsn = ssn.Replace("-", "").Replace(" ", "").Trim();
        
        return Encrypt(cleanedSsn);
    }

    /// <summary>
    /// Decrypts an SSN from storage
    /// </summary>
    public string? DecryptSSN(string? encryptedSsn)
    {
        if (string.IsNullOrWhiteSpace(encryptedSsn))
        {
            return null;
        }

        var decrypted = Decrypt(encryptedSsn);
        
        // Format SSN with dashes for display: 123456789 -> 123-45-6789
        if (decrypted.Length == 9)
        {
            return $"{decrypted.Substring(0, 3)}-{decrypted.Substring(3, 2)}-{decrypted.Substring(5, 4)}";
        }

        return decrypted;
    }
}


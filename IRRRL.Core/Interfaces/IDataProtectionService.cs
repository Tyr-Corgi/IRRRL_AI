namespace IRRRL.Core.Interfaces;

/// <summary>
/// Service for encrypting and decrypting sensitive data (like SSN)
/// </summary>
public interface IDataProtectionService
{
    /// <summary>
    /// Encrypts a plaintext string (e.g., "123-45-6789")
    /// Returns a long encrypted string that's safe to store in database
    /// </summary>
    string Encrypt(string plaintext);
    
    /// <summary>
    /// Decrypts an encrypted string back to plaintext
    /// </summary>
    string Decrypt(string ciphertext);
    
    /// <summary>
    /// Encrypts an SSN and formats it for storage
    /// Handles null/empty values gracefully
    /// </summary>
    string? EncryptSSN(string? ssn);
    
    /// <summary>
    /// Decrypts an SSN from storage
    /// </summary>
    string? DecryptSSN(string? encryptedSsn);
}


using Microsoft.Extensions.Logging;

namespace IRRRL.Infrastructure.Services;

/// <summary>
/// Service for storing and retrieving document files
/// </summary>
public interface IDocumentStorageService
{
    Task<string> SaveDocumentAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<(Stream Stream, string ContentType)> GetDocumentAsync(string filePath, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(string filePath, CancellationToken cancellationToken = default);
    Task<bool> DocumentExistsAsync(string filePath, CancellationToken cancellationToken = default);
}

/// <summary>
/// File system implementation of document storage
/// For production, consider Azure Blob Storage or AWS S3
/// </summary>
public class FileSystemDocumentStorageService : IDocumentStorageService
{
    private readonly string _basePath;
    private readonly ILogger<FileSystemDocumentStorageService> _logger;

    public FileSystemDocumentStorageService(
        ILogger<FileSystemDocumentStorageService> logger,
        string? basePath = null)
    {
        _logger = logger;
        // Default to Documents folder in web root
        _basePath = basePath ?? Path.Combine(Directory.GetCurrentDirectory(), "Documents");
        
        // Ensure directory exists
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
            _logger.LogInformation("Created documents storage directory at {Path}", _basePath);
        }
    }

    public async Task<string> SaveDocumentAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate unique file name to avoid conflicts
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            
            // Create subdirectory by date for organization
            var dateFolder = DateTime.UtcNow.ToString("yyyy-MM");
            var directoryPath = Path.Combine(_basePath, dateFolder);
            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fullPath = Path.Combine(directoryPath, uniqueFileName);
            
            // Save file
            using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
            }

            // Return relative path for database storage
            var relativePath = Path.Combine(dateFolder, uniqueFileName);
            
            _logger.LogInformation(
                "Document saved successfully: {FileName} -> {RelativePath} ({ContentType})",
                fileName, relativePath, contentType);

            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving document: {FileName}", fileName);
            throw;
        }
    }

    public async Task<(Stream Stream, string ContentType)> GetDocumentAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_basePath, filePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Document not found: {filePath}");
            }

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream, cancellationToken);
            }
            
            memoryStream.Position = 0;

            // Determine content type from extension
            var extension = Path.GetExtension(fullPath).ToLowerInvariant();
            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            _logger.LogInformation("Document retrieved successfully: {FilePath}", filePath);

            return (memoryStream, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document: {FilePath}", filePath);
            throw;
        }
    }

    public Task DeleteDocumentAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_basePath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("Document deleted successfully: {FilePath}", filePath);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent document: {FilePath}", filePath);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document: {FilePath}", filePath);
            throw;
        }
    }

    public Task<bool> DocumentExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }
}


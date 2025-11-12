using IRRRL.Core.Entities;
using IRRRL.Core.Enums;
using IRRRL.Infrastructure.Data;
using IRRRL.Infrastructure.Services;
using IRRRL.Web.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Web.Features.Veteran.UploadDocument;

/// <summary>
/// Command to upload a document for an IRRRL application
/// </summary>
public record UploadDocumentCommand(
    int ApplicationId,
    DocumentType DocumentType,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream FileStream,
    string UploadedByUserId
) : ICommand<Result<int>>; // Returns document ID

/// <summary>
/// Handler for uploading documents
/// Saves file to storage and creates database record
/// </summary>
public class UploadDocumentHandler : IRequestHandler<UploadDocumentCommand, Result<int>>
{
    private readonly ApplicationDbContext _context;
    private readonly IDocumentStorageService _documentStorage;
    private readonly ILogger<UploadDocumentHandler> _logger;

    public UploadDocumentHandler(
        ApplicationDbContext context,
        IDocumentStorageService documentStorage,
        ILogger<UploadDocumentHandler> logger)
    {
        _context = context;
        _documentStorage = documentStorage;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(
        UploadDocumentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "User {UserId} uploading document {DocumentType} for application {ApplicationId}",
                request.UploadedByUserId, request.DocumentType, request.ApplicationId);

            // Validate application exists and belongs to user
            var application = await _context.IRRRLApplications
                .Include(a => a.Borrower)
                .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

            if (application == null)
            {
                return Result.Failure<int>("Application not found");
            }

            if (application.Borrower.UserId != request.UploadedByUserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to upload document for application {ApplicationId} belonging to another user",
                    request.UploadedByUserId, request.ApplicationId);
                return Result.Failure<int>("Unauthorized access to application");
            }

            // Validate file
            if (request.FileSizeBytes > 10 * 1024 * 1024) // 10 MB
            {
                return Result.Failure<int>("File size exceeds 10 MB limit");
            }

            var allowedContentTypes = new[]
            {
                "application/pdf",
                "image/jpeg",
                "image/png"
            };

            if (!allowedContentTypes.Contains(request.ContentType.ToLowerInvariant()))
            {
                return Result.Failure<int>("Invalid file type. Only PDF, JPG, and PNG files are allowed.");
            }

            // Check if document of this type already exists
            var existingDocument = await _context.Documents
                .Where(d => d.IRRRLApplicationId == request.ApplicationId 
                         && d.DocumentType == request.DocumentType
                         && d.IsCurrentVersion)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingDocument != null)
            {
                // Mark existing as old version
                existingDocument.IsCurrentVersion = false;
            }

            // Save file to storage
            var filePath = await _documentStorage.SaveDocumentAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                cancellationToken);

            // Create database record
            var document = new Document
            {
                IRRRLApplicationId = request.ApplicationId,
                DocumentType = request.DocumentType,
                FileName = request.FileName,
                FilePath = filePath,
                ContentType = request.ContentType,
                FileSizeBytes = request.FileSizeBytes,
                IsGenerated = false,
                IsValidated = false,
                AIProcessed = false,
                IsComplete = true, // Assume complete until reviewed
                IsLegible = true,  // Assume legible until reviewed
                IsCurrent = true,
                Version = existingDocument != null ? existingDocument.Version + 1 : 1,
                PreviousVersionId = existingDocument?.Id,
                IsCurrentVersion = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Document {DocumentId} uploaded successfully for application {ApplicationId}",
                document.Id, request.ApplicationId);

            // TODO: Trigger AI validation workflow
            // await _mediator.Send(new ValidateDocumentCommand(document.Id), cancellationToken);

            // TODO: Send notification to loan officer
            // await _notificationService.NotifyLoanOfficerOfNewDocument(application.AssignedLoanOfficerId, document.Id);

            return Result.Success(document.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document for application {ApplicationId}", request.ApplicationId);
            return Result.Failure<int>($"An error occurred while uploading the document: {ex.Message}");
        }
    }
}


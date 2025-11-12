using IRRRL.Core.Enums;
using IRRRL.Infrastructure.Data;
using IRRRL.Web.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IRRRL.Web.Features.Veteran.GetMyDocuments;

/// <summary>
/// Query to get all documents for a veteran's application
/// </summary>
public record GetMyDocumentsQuery(
    int ApplicationId,
    string UserId
) : IQuery<Result<List<DocumentDto>>>;

/// <summary>
/// Document DTO for veteran view
/// </summary>
public record DocumentDto(
    int Id,
    DocumentType DocumentType,
    string FileName,
    long FileSizeBytes,
    bool IsValidated,
    DateTime UploadedDate,
    string? ValidationNotes
);

/// <summary>
/// Handler for getting veteran's documents
/// </summary>
public class GetMyDocumentsHandler : IRequestHandler<GetMyDocumentsQuery, Result<List<DocumentDto>>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetMyDocumentsHandler> _logger;

    public GetMyDocumentsHandler(
        ApplicationDbContext context,
        ILogger<GetMyDocumentsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<DocumentDto>>> Handle(
        GetMyDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Verify application belongs to user
            var application = await _context.IRRRLApplications
                .Include(a => a.Borrower)
                .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

            if (application == null)
            {
                return Result.Failure<List<DocumentDto>>("Application not found");
            }

            if (application.Borrower.UserId != request.UserId)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to access documents for application {ApplicationId} belonging to another user",
                    request.UserId, request.ApplicationId);
                return Result.Failure<List<DocumentDto>>("Unauthorized access");
            }

            // Get all current version documents
            var documents = await _context.Documents
                .Where(d => d.IRRRLApplicationId == request.ApplicationId && d.IsCurrentVersion)
                .OrderBy(d => d.DocumentType)
                .Select(d => new DocumentDto(
                    d.Id,
                    d.DocumentType,
                    d.FileName,
                    d.FileSizeBytes,
                    d.IsValidated,
                    d.CreatedAt,
                    d.ValidationNotes
                ))
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} documents for application {ApplicationId}",
                documents.Count, request.ApplicationId);

            return Result.Success(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents for application {ApplicationId}", request.ApplicationId);
            return Result.Failure<List<DocumentDto>>($"An error occurred: {ex.Message}");
        }
    }
}


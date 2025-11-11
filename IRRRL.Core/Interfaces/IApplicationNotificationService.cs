using IRRRL.Core.Enums;

namespace IRRRL.Core.Interfaces;

/// <summary>
/// Service for sending real-time notifications
/// </summary>
public interface IApplicationNotificationService
{
    Task NotifyStatusChangeAsync(int applicationId, ApplicationStatus newStatus, string? notes = null);
    Task NotifyNewActionItemAsync(int applicationId, int actionItemId, string title, string priority);
    Task NotifyActionItemCompletedAsync(int applicationId, int actionItemId);
    Task NotifyDocumentUploadedAsync(int applicationId, int documentId, string documentType);
    Task NotifyDocumentValidatedAsync(int applicationId, int documentId, bool isValid, string? issues = null);
    Task NotifyEligibilityVerifiedAsync(int applicationId, bool isEligible, string summary);
}


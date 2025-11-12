using Orleans;

namespace IRRRL.Core.Grains;

/// <summary>
/// Grain for managing real-time notifications for a specific user
/// Each user gets their own NotificationGrain (identified by userId)
/// 
/// KEY CONCEPT: This demonstrates the "Actor per Entity" pattern
/// - One grain per user for notifications
/// - Grain handles SignalR connection management
/// - Persists notification history
/// - Can push notifications to connected clients
/// </summary>
public interface INotificationGrain : IGrainWithStringKey  // Key = UserId
{
    /// <summary>
    /// Register a SignalR connection for this user
    /// Orleans Grain will track active connections
    /// </summary>
    Task RegisterConnectionAsync(string connectionId);
    
    /// <summary>
    /// Unregister a SignalR connection
    /// </summary>
    Task UnregisterConnectionAsync(string connectionId);
    
    /// <summary>
    /// Send a notification to this user
    /// Grain will push to all active connections via SignalR
    /// </summary>
    Task SendNotificationAsync(string title, string message, NotificationType type);
    
    /// <summary>
    /// Get recent notifications for this user
    /// </summary>
    Task<List<NotificationDto>> GetRecentNotificationsAsync(int count = 10);
    
    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    Task MarkAllAsReadAsync();
}

/// <summary>
/// Notification types
/// </summary>
public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    ApplicationUpdate,
    DocumentReceived,
    ActionItemCompleted
}

/// <summary>
/// DTO for notification
/// </summary>
public record NotificationDto(
    string Id,
    string Title,
    string Message,
    NotificationType Type,
    DateTime CreatedAt,
    bool IsRead
);


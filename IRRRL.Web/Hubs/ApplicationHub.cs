using IRRRL.Core.Grains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Orleans;

namespace IRRRL.Web.Hubs;

/// <summary>
/// SignalR hub for real-time application updates
/// NOW INTEGRATED WITH ORLEANS!
/// 
/// Pattern: SignalR Hub + Orleans Grains
/// - Hub handles WebSocket connections
/// - Grains handle business logic and state
/// - Grains push notifications to Hub
/// - Scalable across multiple servers
/// </summary>
[Authorize]
public class ApplicationHub : Hub
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<ApplicationHub> _logger;

    public ApplicationHub(IGrainFactory grainFactory, ILogger<ApplicationHub> logger)
    {
        _grainFactory = grainFactory;
        _logger = logger;
    }

    /// <summary>
    /// Subscribe to updates for a specific application
    /// </summary>
    public async Task SubscribeToApplication(int applicationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Application_{applicationId}");
        _logger.LogInformation("User {UserId} subscribed to application {ApplicationId}", Context.UserIdentifier, applicationId);
    }
    
    /// <summary>
    /// Unsubscribe from application updates
    /// </summary>
    public async Task UnsubscribeFromApplication(int applicationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Application_{applicationId}");
        _logger.LogInformation("User {UserId} unsubscribed from application {ApplicationId}", Context.UserIdentifier, applicationId);
    }
    
    /// <summary>
    /// Subscribe to all applications (for loan officers/underwriters)
    /// </summary>
    public async Task SubscribeToAllApplications()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "AllApplications");
        _logger.LogInformation("User {UserId} subscribed to all applications", Context.UserIdentifier);
    }
    
    /// <summary>
    /// Unsubscribe from all applications
    /// </summary>
    public async Task UnsubscribeFromAllApplications()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AllApplications");
        _logger.LogInformation("User {UserId} unsubscribed from all applications", Context.UserIdentifier);
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);
            
            // Register connection with the user's NotificationGrain (Orleans!)
            var notificationGrain = _grainFactory.GetGrain<INotificationGrain>(userId);
            await notificationGrain.RegisterConnectionAsync(Context.ConnectionId);
        }
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("User {UserId} disconnected (connection {ConnectionId})", userId, Context.ConnectionId);
            
            // Unregister connection from NotificationGrain (Orleans!)
            var notificationGrain = _grainFactory.GetGrain<INotificationGrain>(userId);
            await notificationGrain.UnregisterConnectionAsync(Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}


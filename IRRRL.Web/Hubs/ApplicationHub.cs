using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IRRRL.Web.Hubs;

/// <summary>
/// SignalR hub for real-time application updates
/// </summary>
[Authorize]
public class ApplicationHub : Hub
{
    /// <summary>
    /// Subscribe to updates for a specific application
    /// </summary>
    public async Task SubscribeToApplication(int applicationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Application_{applicationId}");
    }
    
    /// <summary>
    /// Unsubscribe from application updates
    /// </summary>
    public async Task UnsubscribeFromApplication(int applicationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Application_{applicationId}");
    }
    
    /// <summary>
    /// Subscribe to all applications (for loan officers/underwriters)
    /// </summary>
    public async Task SubscribeToAllApplications()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "AllApplications");
    }
    
    /// <summary>
    /// Unsubscribe from all applications
    /// </summary>
    public async Task UnsubscribeFromAllApplications()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AllApplications");
    }
    
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        
        // Optionally log connection
        var userId = Context.UserIdentifier;
        // Log connection
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        
        // Optionally log disconnection
        var userId = Context.UserIdentifier;
        // Log disconnection
    }
}


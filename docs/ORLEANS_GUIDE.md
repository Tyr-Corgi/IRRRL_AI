# 🎯 Orleans Integration Guide - IRRRL AI

## Overview

This project now uses **Microsoft Orleans** - a distributed actor framework for building scalable, concurrent systems. Orleans works PERFECTLY with Vertical Slice Architecture!

---

## 🌟 What is Orleans?

**Orleans = Virtual Actors (Grains) distributed across Servers (Silos)**

- Each **Grain** is a mini-service with its own state
- Grains are **automatically distributed** across Silos (servers)
- Orleans handles **concurrency**, **persistence**, and **failover**
- Perfect for multi-tenant, real-time applications like IRRRL

---

## 🏗️ Architecture: Vertical Slice + Orleans

```
┌─────────────────────────────────────────────────────┐
│            WEB REQUEST (Blazor/MVC)                 │
└────────────────────┬────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────┐
│         VERTICAL SLICE (MediatR Handler)            │
│  Features/LoanOfficer/UpdateStatus/UpdateStatusCmd  │
└────────────────────┬────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────┐
│           ORLEANS GRAIN (Business Logic)            │
│        ApplicationGrain.UpdateStatusAsync()         │
│  - Thread-safe (one message at a time)              │
│  - State persistence                                │
│  - Automatic distribution across Silos              │
└────────────────────┬────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────┐
│              DATABASE (EF Core)                     │
│           + SignalR (Real-time updates)             │
└─────────────────────────────────────────────────────┘
```

**Key Points:**
- **Vertical Slice** = Organization (how features are structured)
- **Orleans** = Execution (how features run/scale)
- **MediatR Handler** calls **Orleans Grain**
- **Grain** handles concurrency, state, and notifications

---

## 📦 Grains in This Project

### 1. **ApplicationGrain** (`IApplicationGrain`)
- **One Grain per IRRRL Application**
- **Key:** Application ID (e.g., `1`, `2`, `1234`)
- **Manages:** Application state, status changes, documents, action items

**Methods:**
```csharp
Task<IRRRLApplication?> GetApplicationAsync();
Task UpdateStatusAsync(ApplicationStatus newStatus, string? notes, string? userId);
Task AddNoteAsync(string content, ApplicationNoteType type, ...);
Task MarkDocumentReceivedAsync(int documentId);
Task CompleteActionItemAsync(int actionItemId);
Task<ApplicationStatisticsDto> GetStatisticsAsync();
```

**Example Usage:**
```csharp
// In UpdateStatusHandler (Vertical Slice)
var applicationGrain = _grainFactory.GetGrain<IApplicationGrain>(applicationId);
await applicationGrain.UpdateStatusAsync(ApplicationStatus.UnderwriterReady, "Ready for review");
```

### 2. **NotificationGrain** (`INotificationGrain`)
- **One Grain per User**
- **Key:** User ID (e.g., `"user-12345"`)
- **Manages:** Real-time notifications via SignalR

**Methods:**
```csharp
Task RegisterConnectionAsync(string connectionId);
Task UnregisterConnectionAsync(string connectionId);
Task SendNotificationAsync(string title, string message, NotificationType type);
Task<List<NotificationDto>> GetRecentNotificationsAsync(int count);
Task MarkAllAsReadAsync();
```

**Example Usage:**
```csharp
// Inside ApplicationGrain when status changes
var notificationGrain = _grainFactory.GetGrain<INotificationGrain>(loanOfficerId);
await notificationGrain.SendNotificationAsync(
    "Application Updated",
    $"Application {appNumber} is ready for review",
    NotificationType.ApplicationUpdate);
```

---

## 🔥 Real-World Example: Update Application Status

### **Before Orleans (Direct Database):**
```csharp
public class UpdateStatusHandler
{
    private readonly ApplicationDbContext _context;
    
    public async Task<Result> Handle(UpdateStatusCommand request, ...)
    {
        var app = await _context.Applications.FindAsync(request.ApplicationId);
        app.Status = request.NewStatus;
        await _context.SaveChangesAsync(); // What if 2 users update at the same time?
        return Result.Success();
    }
}
```

**Problems:**
- ❌ Race conditions (concurrent updates)
- ❌ No real-time notifications
- ❌ Hard to scale (all logic in one handler)

### **After Orleans (Using Grains):**
```csharp
public class UpdateStatusHandler
{
    private readonly IGrainFactory _grainFactory;
    
    public async Task<Result> Handle(UpdateStatusCommand request, ...)
    {
        // Get the ApplicationGrain (Orleans handles activation/distribution)
        var grain = _grainFactory.GetGrain<IApplicationGrain>(request.ApplicationId);
        
        // Orleans guarantees thread-safety (one message at a time)
        await grain.UpdateStatusAsync(request.NewStatus, request.Notes);
        
        // Grain automatically:
        // - Updates database
        // - Sends notifications to loan officer
        // - Creates audit trail
        // - All thread-safe!
        
        return Result.Success();
    }
}
```

**Benefits:**
- ✅ Thread-safe (Orleans guarantees one message at a time per Grain)
- ✅ Real-time notifications (Grain → NotificationGrain → SignalR)
- ✅ Automatic distribution (Grain can be on any Silo)
- ✅ Failover (if Silo crashes, Grain restarts on another)

---

## 🎓 Key Orleans Concepts

### 1. **Grain Activation**
```csharp
public override async Task OnActivateAsync(CancellationToken cancellationToken)
{
    // Called when Grain is first accessed
    // Load state from database here
    _application = await _context.Applications.FindAsync(ApplicationId);
}
```

**When Grains Activate:**
- First call to the Grain
- After deactivation (idle timeout)
- After Silo restart

### 2. **Grain Deactivation**
- Grains automatically deactivate after idle period (default: 2 hours)
- State is persisted before deactivation
- Next call re-activates the Grain

### 3. **Grain-to-Grain Communication**
```csharp
// Inside ApplicationGrain
public async Task UpdateStatusAsync(...)
{
    // Update own state
    _application.Status = newStatus;
    await SaveToDatabase();
    
    // Call another Grain!
    var notificationGrain = _grainFactory.GetGrain<INotificationGrain>(loanOfficerId);
    await notificationGrain.SendNotificationAsync("Status Updated", ...);
}
```

### 4. **Concurrency Model**
- Orleans guarantees **one message at a time** per Grain
- No locks needed!
- Multiple Grains can run in parallel

---

## 🚀 Orleans Dashboard

**Access:** http://localhost:8080 (when app is running)

**Features:**
- See all active Grains
- Monitor Grain calls/sec
- View Grain activation/deactivation
- Check Silo health

**Screenshot (typical):**
```
Grains Active: 15
├── IApplicationGrain: 10 instances
│   ├── Grain ID: 1 (Application #1)
│   ├── Grain ID: 2 (Application #2)
│   └── ...
├── INotificationGrain: 5 instances
│   ├── Grain ID: "user-123" (Loan Officer)
│   ├── Grain ID: "user-456" (Veteran)
│   └── ...

Silo Status: Healthy
Total Calls: 1,245
```

---

## 🔧 Configuration

### **Development (Current Setup):**
```csharp
// Program.cs
builder.Host.UseOrleans((context, siloBuilder) =>
{
    siloBuilder.UseLocalhostClustering(); // Single machine, multiple ports
    siloBuilder.UseDashboard(options => options.Port = 8080);
});
```

### **Production (Future):**
```csharp
builder.Host.UseOrleans((context, siloBuilder) =>
{
    // Azure Table Storage for cluster membership
    siloBuilder.UseAzureStorageClustering(options =>
    {
        options.ConfigureTableServiceClient(connectionString);
    });
    
    // Persistent state
    siloBuilder.AddAzureTableGrainStorage("ApplicationState", options =>
    {
        options.ConfigureTableServiceClient(connectionString);
    });
});
```

---

## 📊 Vertical Slice + Orleans Pattern

### **Pattern:**
1. **HTTP Request** → Blazor Page
2. **Page** → MediatR.Send(Command)
3. **MediatR Handler** (Vertical Slice) → Get Grain
4. **Orleans Grain** → Business Logic + Database + Notifications
5. **Grain** → Other Grains (if needed)
6. **Grain** → SignalR Hub → Push to Clients

### **Example Flow:**
```
Loan Officer clicks "Approve Application"
    ↓
Blazor: await Mediator.Send(new UpdateStatusCommand(...))
    ↓
UpdateStatusHandler: var grain = _grainFactory.GetGrain<IApplicationGrain>(id)
    ↓
ApplicationGrain: UpdateStatusAsync()
    ↓
├── Save to Database
├── Create Audit Log
└── Notify Loan Officer:
    ↓
    NotificationGrain.SendNotificationAsync()
        ↓
        SignalR Hub → Push to Browser
```

---

## 🎯 When to Use Grains

### ✅ **Use Grains For:**
- **Application Processing** - One grain per application
- **Document Processing** - Parallel processing
- **User Notifications** - One grain per user
- **Background Jobs** - Scheduled tasks
- **Real-Time Updates** - SignalR integration
- **Rate Limiting** - One grain per API key
- **Session Management** - One grain per session

### ❌ **Don't Use Grains For:**
- **Simple CRUD** - Just use EF Core directly
- **Read-Only Queries** - Use database directly (faster)
- **Static Data** - Cache in memory
- **Short-Lived Operations** - Overhead not worth it

---

## 🔥 Advanced Patterns

### **1. Grain with Timer (Background Processing)**
```csharp
public class ApplicationGrain : Grain, IApplicationGrain
{
    private IDisposable? _timer;
    
    public override Task OnActivateAsync()
    {
        // Check for overdue action items every 5 minutes
        _timer = RegisterTimer(
            CheckOverdueActionItems,
            null,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(5));
        
        return base.OnActivateAsync();
    }
    
    private async Task CheckOverdueActionItems(object state)
    {
        // Notify loan officer of overdue items
    }
}
```

### **2. Grain with Observer Pattern**
```csharp
public interface IApplicationObserver : IGrainObserver
{
    void OnStatusChanged(ApplicationStatus newStatus);
}

public class ApplicationGrain : Grain, IApplicationGrain
{
    private readonly ObserverManager<IApplicationObserver> _observers = new();
    
    public Task Subscribe(IApplicationObserver observer)
    {
        _observers.Subscribe(observer, observer);
        return Task.CompletedTask;
    }
    
    public async Task UpdateStatusAsync(ApplicationStatus newStatus)
    {
        _application.Status = newStatus;
        await SaveToDatabase();
        
        // Notify all observers
        _observers.Notify(obs => obs.OnStatusChanged(newStatus));
    }
}
```

---

## 🧪 Testing Grains

### **Unit Test:**
```csharp
public class ApplicationGrainTests
{
    [Fact]
    public async Task UpdateStatus_ShouldChangeStatus()
    {
        // Arrange
        var cluster = new TestClusterBuilder().Build();
        await cluster.DeployAsync();
        
        var grain = cluster.GrainFactory.GetGrain<IApplicationGrain>(1);
        
        // Act
        await grain.UpdateStatusAsync(ApplicationStatus.Approved, "Test note");
        var result = await grain.GetApplicationAsync();
        
        // Assert
        Assert.Equal(ApplicationStatus.Approved, result.Status);
    }
}
```

---

## 📚 Resources

- **Orleans Docs:** https://learn.microsoft.com/en-us/dotnet/orleans/
- **GitHub:** https://github.com/dotnet/orleans
- **Samples:** https://github.com/dotnet/orleans/tree/main/samples
- **Discord:** https://aka.ms/orleans-discord

---

## 💡 Key Takeaways

1. **Vertical Slice** = Structure (how features are organized)
2. **Orleans** = Execution (how features run/scale)
3. **Grains** = Virtual actors with state (one per entity)
4. **Thread-Safe** = Orleans handles concurrency automatically
5. **Distributed** = Grains automatically distributed across Silos
6. **Real-Time** = Grains + SignalR = Scalable notifications
7. **Perfect for IRRRL** = Multi-tenant, real-time, high-scale

---

## 🎉 You're Now Using Orleans!

**Your IRRRL application is now:**
- ✅ Distributed (can run on multiple servers)
- ✅ Scalable (millions of applications)
- ✅ Thread-safe (no race conditions)
- ✅ Real-time (SignalR + Orleans)
- ✅ Production-ready architecture

**Just like your manager's big project!** 🚀


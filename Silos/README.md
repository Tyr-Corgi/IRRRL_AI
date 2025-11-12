# 🏗️ Orleans Silos Directory

## Overview

This directory is for **multi-Silo deployment** of the IRRRL AI application using Microsoft Orleans.

---

## 📊 Current Architecture (Single Silo)

Right now, the **IRRRL.Web** project acts as both:
- **Web Server** (Blazor UI, API endpoints)
- **Orleans Silo** (hosts Grains)

This is perfect for **development** and **small deployments**.

---

## 🚀 Future Multi-Silo Architecture

When you need to scale, you can split into multiple Silos:

```
┌─────────────────────────────────┐
│      Web Silo (IRRRL.Web)       │  ← User-facing
│  - Blazor UI                    │
│  - API Endpoints                │
│  - SignalR Hub                  │
│  - Orleans Client               │
└─────────────────────────────────┘
           ↓ (calls Grains)
┌─────────────────────────────────┐
│   Worker Silo 1 (Background)    │  ← Processing
│  - ApplicationGrain             │
│  - NotificationGrain            │
│  - Document Processing          │
│  - AI Processing                │
└─────────────────────────────────┘

┌─────────────────────────────────┐
│   Worker Silo 2 (Background)    │  ← More processing
│  - ApplicationGrain             │
│  - NotificationGrain            │
│  - Scheduled Tasks              │
│  - Reporting                    │
└─────────────────────────────────┘
```

---

## 📁 Future Silo Projects

### **Silos/IRRRL.Silo.Worker/**
- Dedicated background processing Silo
- Hosts all Grains (ApplicationGrain, NotificationGrain, etc.)
- No web UI
- Can scale independently (multiple instances)

### **Silos/IRRRL.Silo.Reports/**
- Dedicated reporting Silo
- Heavy data processing
- Scheduled reports
- Analytics Grains

### **Silos/IRRRL.Silo.AI/**
- AI-specific processing
- Document analysis Grains
- Action item generation Grains
- Can use GPU servers

---

## 🔧 How to Create a Worker Silo

### **1. Create Console App:**
```bash
cd Silos
dotnet new console -n IRRRL.Silo.Worker
cd IRRRL.Silo.Worker
dotnet add reference ../../IRRRL.Core
dotnet add reference ../../IRRRL.Infrastructure
dotnet add package Microsoft.Orleans.Server
```

### **2. Program.cs:**
```csharp
using Orleans;
using Orleans.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        // Use Azure Table Storage for clustering (production)
        siloBuilder.UseAzureStorageClustering(options =>
        {
            options.ConfigureTableServiceClient(connectionString);
        });
        
        // Or localhost for dev
        siloBuilder.UseLocalhostClustering(
            siloPort: 11112,
            gatewayPort: 30001);
        
        // Configure Grain storage
        siloBuilder.AddAzureTableGrainStorage("ApplicationState", options =>
        {
            options.ConfigureTableServiceClient(connectionString);
        });
    });

var host = builder.Build();
await host.RunAsync();
```

### **3. Update IRRRL.Web to be Orleans Client:**
```csharp
// In Program.cs, change from:
builder.Host.UseOrleans(...)

// To:
builder.Host.UseOrleansClient((context, clientBuilder) =>
{
    clientBuilder.UseLocalhostClustering();
    // Or Azure Table Storage for prod
});
```

---

## 🎯 When to Split into Multiple Silos

### **Keep Single Silo If:**
- < 1,000 concurrent users
- < 10,000 applications
- Simple processing
- Single server deployment

### **Split into Multiple Silos When:**
- \> 1,000 concurrent users
- Heavy background processing
- AI/ML workloads
- Need independent scaling
- Multiple geographic regions
- High availability requirements

---

## 📊 Production Deployment Examples

### **Small Deployment (1 Silo):**
```
Azure App Service
└── IRRRL.Web (Single Silo)
    ├── Blazor UI
    └── Orleans Grains
```

### **Medium Deployment (2-3 Silos):**
```
Azure App Service
├── IRRRL.Web (Client + Silo)
└── Azure Container Instance
    ├── Worker Silo 1
    └── Worker Silo 2
```

### **Large Deployment (5+ Silos):**
```
Azure Kubernetes Service (AKS)
├── Web Pods (3 replicas)
│   └── IRRRL.Web
├── Worker Pods (5 replicas)
│   └── IRRRL.Silo.Worker
└── AI Pods (2 replicas with GPU)
    └── IRRRL.Silo.AI
```

---

## 🔗 Cluster Membership

All Silos must use the same **cluster membership provider**:

### **Development:**
- `UseLocalhostClustering()` - Each Silo on different port

### **Production Options:**
- **Azure Table Storage** - Best for Azure deployments
- **AWS DynamoDB** - Best for AWS deployments
- **SQL Server** - Any environment with SQL
- **Consul** - Service discovery

---

## 📚 Resources

- [Orleans Deployment Guide](https://learn.microsoft.com/en-us/dotnet/orleans/deployment/)
- [Multi-Silo Deployment](https://learn.microsoft.com/en-us/dotnet/orleans/deployment/multi-cluster)
- [Azure Deployment](https://learn.microsoft.com/en-us/dotnet/orleans/deployment/deploy-to-azure-app-service)

---

## 💡 Current Status

**✅ Single Silo Setup Complete**
- IRRRL.Web acts as single Silo
- Perfect for development and small deployments
- Orleans Dashboard available at http://localhost:8080

**🔜 Future: Multi-Silo Ready**
- This folder is prepared for future Worker Silos
- Can split anytime when scaling is needed
- Architecture supports it already!

---

**Note:** For your learning with your manager's project, understanding this structure is KEY! Many production Orleans applications use this multi-Silo pattern.


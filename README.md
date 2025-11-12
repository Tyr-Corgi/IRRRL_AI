# 🏢 IRRRL AI - VA Streamline Refinance Processing System

A comprehensive .NET 8 application for processing VA Interest Rate Reduction Refinance Loans (IRRRLs) using **Vertical Slice Architecture** and **Microsoft Orleans** for distributed processing.

---

## 🎯 Overview

This application streamlines the IRRRL (VA Streamline Refinance) process by:
- ✅ Automating eligibility verification
- ✅ Calculating Net Tangible Benefit (NTB)
- ✅ Managing document collection
- ✅ Generating VA-compliant forms
- ✅ Providing real-time status updates
- ✅ Supporting multi-role workflows (Veteran, Loan Officer, Underwriter, Processor)

---

## 🏗️ Architecture

### **Vertical Slice Architecture + Orleans**

This project uses **cutting-edge architectural patterns**:

1. **Vertical Slice Architecture** - Features organized by use case, not technical layer
2. **Microsoft Orleans** - Distributed actor framework for scalability
3. **MediatR** - CQRS pattern for clean command/query separation
4. **SignalR** - Real-time updates
5. **Entity Framework Core** - Database persistence
6. **ASP.NET Core Identity** - Role-based authentication

```
┌─────────────────────────────────────┐
│     Web Request (Blazor/MVC)        │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│  Vertical Slice (MediatR Handler)   │
│  Features/LoanOfficer/UpdateStatus/ │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│    Orleans Grain (Business Logic)   │
│   ApplicationGrain.UpdateStatus()   │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│   Database (EF Core) + SignalR      │
└─────────────────────────────────────┘
```

---

## 📁 Project Structure

```
IRRRL AI/
├── docs/                           # 📚 All documentation
│   ├── ORLEANS_GUIDE.md           # Orleans integration guide
│   ├── VERTICAL_SLICE_GUIDE.md    # Vertical Slice patterns
│   ├── DATABASE_SETUP.md          # Database setup instructions
│   ├── GETTING_STARTED.md         # Quick start guide
│   └── ...                         # Other guides
│
├── Silos/                          # 🏗️ Multi-Silo deployment (future)
│   └── README.md                  # How to scale with multiple Silos
│
├── IRRRL.Core/                    # 🎯 Domain logic & Grain interfaces
│   ├── Entities/                  # Domain entities
│   ├── Enums/                     # Enumerations
│   ├── Grains/                    # Orleans Grain interfaces
│   │   ├── IApplicationGrain.cs   # Application management
│   │   └── INotificationGrain.cs  # Real-time notifications
│   ├── Interfaces/                # Service contracts
│   ├── Services/                  # Core business services
│   └── Validators/                # Validation logic
│
├── IRRRL.Infrastructure/          # 🔧 Implementations
│   ├── AI/                        # AI services (OpenAI integration)
│   ├── Data/                      # Database context & migrations
│   ├── Documents/                 # PDF generation (QuestPDF)
│   ├── Grains/                    # Orleans Grain implementations
│   │   └── ApplicationGrain.cs    # Application state management
│   ├── Migrations/                # EF Core migrations
│   └── Services/                  # Infrastructure services
│
├── IRRRL.Web/                     # 🌐 Blazor Server UI
│   ├── Features/                  # Vertical Slices
│   │   ├── Common/                # Shared abstractions
│   │   │   ├── IQuery.cs          # Query marker
│   │   │   ├── ICommand.cs        # Command marker
│   │   │   ├── Result.cs          # Result pattern
│   │   │   └── Behaviors/         # MediatR pipeline behaviors
│   │   ├── LoanOfficer/           # Loan Officer features
│   │   │   ├── GetDashboard/      # Dashboard query
│   │   │   ├── GetApplicationDetail/
│   │   │   ├── AddNote/           # Add note command
│   │   │   └── UpdateStatus/      # Update status command
│   │   ├── Veteran/               # Veteran features
│   │   │   └── SubmitApplication/
│   │   └── Underwriter/           # Underwriter features
│   │       └── GetQueue/
│   ├── Hubs/                      # SignalR hubs
│   ├── Pages/                     # Blazor pages
│   ├── Components/                # Reusable components
│   └── wwwroot/                   # Static files
│
├── IRRRL.Shared/                  # 📦 Shared code
│   ├── Constants/
│   └── DTOs/
│
├── IRRRL.Tests/                   # 🧪 Unit & Integration tests
│   ├── Unit/
│   └── Integration/
│
└── README.md                      # This file!
```

---

## 🚀 Quick Start

### **Prerequisites:**
- .NET 8 SDK
- Visual Studio 2022 or VS Code

### **Run the Application:**

```bash
# Clone the repository
git clone https://github.com/Tyr-Corgi/IRRRL_AI.git
cd "IRRRL AI"

# Run the application
cd IRRRL.Web
dotnet run
```

### **Access:**
- **Application:** https://localhost:5001
- **Orleans Dashboard:** http://localhost:8080

### **Login Accounts:**

| Role | Email | Password |
|------|-------|----------|
| **Loan Officer** | loanofficer@irrrl.local | LoanOfficer@123! |
| **Veteran** | veteran@irrrl.local | Veteran@123! |
| **Administrator** | admin@irrrl.local | Admin@123! |

---

## 🎓 Learning Resources

### **For Understanding the Architecture:**

1. **Start Here:** [`docs/GETTING_STARTED.md`](docs/GETTING_STARTED.md)
2. **Vertical Slice:** [`docs/VERTICAL_SLICE_GUIDE.md`](docs/VERTICAL_SLICE_GUIDE.md)
3. **Orleans:** [`docs/ORLEANS_GUIDE.md`](docs/ORLEANS_GUIDE.md)
4. **Database Setup:** [`docs/DATABASE_SETUP.md`](docs/DATABASE_SETUP.md)

### **Key Concepts:**

#### **1. Vertical Slice Architecture**
- Features organized by **use case** (not technical layer)
- Each feature in ONE folder (Query/Command + Handler + DTOs)
- Minimal coupling between features
- Perfect for multi-role systems

#### **2. Orleans (Distributed Actors)**
- **Grains** = Virtual actors with state (one per application, one per user)
- **Silos** = Servers hosting Grains
- Thread-safe, scalable, distributed
- Used by Xbox Live, Halo, Fortune 500 companies

#### **3. CQRS Pattern**
- **Queries** (`IQuery<T>`) - Read operations
- **Commands** (`ICommand<T>`) - Write operations
- Clear separation of concerns

---

## 🔥 Key Features

### **Multi-Role Support:**
- ✅ **Veterans** - Submit applications, upload documents
- ✅ **Loan Officers** - Review applications, manage workflow
- ✅ **Underwriters** - Approval queue, risk assessment
- ✅ **Processors** - Document processing, compliance checks

### **Real-Time Updates:**
- ✅ SignalR integration with Orleans Grains
- ✅ Instant notifications for status changes
- ✅ Live application tracking

### **Scalability:**
- ✅ Orleans Grains distribute across servers automatically
- ✅ Thread-safe concurrent operations
- ✅ Can scale to millions of applications
- ✅ Failover and fault tolerance built-in

### **Security:**
- ✅ ASP.NET Core Identity with role-based authorization
- ✅ Data Protection API for encrypting sensitive data (SSN)
- ✅ Secure authentication cookies

---

## 🛠️ Development

### **Reset Database:**
```bash
cd IRRRL.Web
.\reset-database.cmd
dotnet run
```

### **Run Tests:**
```bash
cd IRRRL.Tests
dotnet test
```

### **Create New Feature (Vertical Slice):**

1. Create folder: `Features/{Role}/{FeatureName}/`
2. Create Query or Command:
```csharp
public record MyQuery(...) : IQuery<MyResult>;
public class MyHandler : IRequestHandler<MyQuery, MyResult> { }
```
3. Use in Blazor page:
```csharp
var result = await Mediator.Send(new MyQuery(...));
```

### **Create New Grain:**

1. Interface in `IRRRL.Core/Grains/IMyGrain.cs`
2. Implementation in `IRRRL.Infrastructure/Grains/MyGrain.cs`
3. Use via `IGrainFactory`:
```csharp
var grain = _grainFactory.GetGrain<IMyGrain>(id);
await grain.DoSomethingAsync();
```

---

## 📊 Orleans Dashboard

When the app is running, visit **http://localhost:8080** to see:
- Active Grains
- Grain activations/deactivations
- Calls per second
- Silo health

---

## 🚀 Deployment

### **Single Silo (Current):**
- Perfect for development and small deployments
- IRRRL.Web acts as both web server and Orleans Silo

### **Multi-Silo (Future):**
- See [`Silos/README.md`](Silos/README.md) for scaling instructions
- Can separate Web Silo from Worker Silos
- Deploy to Azure, AWS, or Kubernetes

---

## 🤝 Contributing

This is a learning project demonstrating enterprise architecture patterns. Feel free to explore and learn from:
- Vertical Slice Architecture implementation
- Orleans distributed actor framework
- MediatR CQRS pattern
- Clean Architecture principles
- Real-time SignalR integration

---

## 📚 Technology Stack

- **.NET 8** - Latest framework
- **Blazor Server** - Interactive web UI
- **Microsoft Orleans 8.2** - Distributed actor framework
- **Entity Framework Core** - ORM
- **SQLite** - Database (dev), SQL Server ready (prod)
- **MediatR** - CQRS implementation
- **SignalR** - Real-time communication
- **QuestPDF** - PDF generation
- **xUnit** - Testing framework
- **FluentValidation** - Input validation
- **Serilog** - Logging

---

## 📖 Documentation

All documentation is in the [`docs/`](docs/) folder:
- Architecture guides
- Setup instructions
- Testing guides
- Implementation notes
- API documentation

---

## 🎯 Project Goals

This project demonstrates:
1. ✅ **Modern .NET Architecture** - Vertical Slice + Orleans
2. ✅ **Enterprise Patterns** - CQRS, DDD, Clean Architecture
3. ✅ **Scalability** - Distributed actors, multi-Silo ready
4. ✅ **Real-World Application** - VA loan processing
5. ✅ **Best Practices** - Testing, documentation, security

---

## 📝 License

This is a learning/demonstration project. Use as reference for your own projects!

---

## 🙏 Acknowledgments

- **Microsoft Orleans** - Amazing distributed actor framework
- **Jimmy Bogard** - Vertical Slice Architecture & MediatR
- **VA IRRRL Program** - Inspiration for domain modeling

---

**Built with ❤️ to learn Vertical Slice Architecture + Orleans for enterprise applications**

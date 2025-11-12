# 🎯 Vertical Slice Architecture Guide

## Overview

This IRRRL AI application uses **Vertical Slice Architecture** to organize code by features rather than technical layers. Each feature is self-contained and includes everything needed for that specific use case.

---

## 🏗️ Why Vertical Slice?

### Traditional Layered Architecture (What We DON'T Have)
```
Controllers/
├── ApplicationController.cs
├── BorrowerController.cs
└── DocumentController.cs
Services/
├── ApplicationService.cs
├── BorrowerService.cs
└── DocumentService.cs
Repositories/
├── ApplicationRepository.cs
├── BorrowerRepository.cs
└── DocumentRepository.cs
```

**Problems:**
- Features are scattered across layers
- Hard to find related code
- Unnecessary abstractions (Repository of Everything)
- High coupling between features
- Changes ripple through all layers

### Vertical Slice Architecture (What We HAVE)
```
Features/
├── LoanOfficer/
│   ├── GetDashboard/
│   │   └── GetDashboardQuery.cs    (Query + Handler + DTOs - EVERYTHING)
│   ├── GetApplicationDetail/
│   │   └── GetApplicationDetailQuery.cs
│   └── AddNote/
│       └── AddNoteCommand.cs
├── Veteran/
│   └── SubmitApplication/
│       └── SubmitApplicationCommand.cs
└── Underwriter/
    └── GetQueue/
        └── GetQueueQuery.cs
```

**Benefits:**
- ✅ Everything for a feature in ONE place
- ✅ Easy to find code ("Where's Add Note?" → `Features/LoanOfficer/AddNote/`)
- ✅ Features don't depend on each other
- ✅ New team members can work on isolated features
- ✅ Delete a feature = Delete one folder
- ✅ Perfect for multi-role systems (Veteran, LoanOfficer, Underwriter, Processor)

---

## 📐 Architecture Principles

### 1. Feature Organization by Role

Each user role has its own features folder:

- **`LoanOfficer/`** - Dashboard, application review, document management, notes
- **`Veteran/`** - Application submission, document upload, status tracking
- **`Underwriter/`** - Queue management, approval workflow
- **`Processor/`** - Document processing, compliance checks

### 2. Queries vs Commands (CQRS)

**Queries** - Read operations (no side effects)
```csharp
public record GetDashboardQuery(string? LoanOfficerId) : IQuery<GetDashboardResult>;
```

**Commands** - Write operations (change state)
```csharp
public record AddNoteCommand(
    int ApplicationId,
    string Content,
    // ... more fields
) : ICommand<Result<ApplicationNote>>;
```

### 3. Self-Contained Features

Each feature folder contains **everything**:
- Query/Command (the request)
- Handler (the business logic)
- DTOs/View Models (the response)
- Validators (optional - using FluentValidation)
- Any feature-specific services

**Example: `GetDashboard/GetDashboardQuery.cs`**
```csharp
// Query - What we're asking for
public record GetDashboardQuery(string? LoanOfficerId) : IQuery<GetDashboardResult>;

// Result - What we get back
public record GetDashboardResult(
    List<ApplicationSummaryDto> Applications,
    DashboardStatistics Statistics
);

// DTOs - Only the data this feature needs
public record ApplicationSummaryDto(
    int Id,
    string ApplicationNumber,
    string VeteranName,
    // ... other fields
);

// Handler - All the logic in ONE place
public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, GetDashboardResult>
{
    public async Task<GetDashboardResult> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        // Load from database
        // Calculate statistics
        // Return result
        // NO service layer! All logic HERE.
    }
}
```

### 4. No Shared View Models Between Features

**❌ WRONG (Tight Coupling):**
```csharp
// Shared/ApplicationDto.cs
public class ApplicationDto // Used by everyone!
{
    // 50 fields that Veteran, LoanOfficer, AND Underwriter all share
}
```

**✅ RIGHT (Independent Features):**
```csharp
// LoanOfficer/GetDashboard/GetDashboardQuery.cs
public record ApplicationSummaryDto(
    int Id,
    string ApplicationNumber,
    string VeteranName,
    ApplicationStatus Status
    // Only fields LoanOfficer dashboard needs
);

// Underwriter/GetQueue/GetQueueQuery.cs
public record UnderwriterQueueItem(
    int ApplicationId,
    string ApplicationNumber,
    bool MeetsNTB,
    bool HasAllDocuments,
    string Priority
    // Different fields! Underwriter needs different data!
);
```

---

## 🚀 How to Use

### Creating a New Feature

**Step 1: Create Feature Folder**
```bash
mkdir Features/LoanOfficer/ExportToPDF
```

**Step 2: Create Query or Command**
```csharp
// Features/LoanOfficer/ExportToPDF/ExportToPDFCommand.cs
using IRRRL.Web.Features.Common;
using MediatR;

namespace IRRRL.Web.Features.LoanOfficer.ExportToPDF;

public record ExportToPDFCommand(int ApplicationId) : ICommand<Result<byte[]>>;

public class ExportToPDFHandler : IRequestHandler<ExportToPDFCommand, Result<byte[]>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExportToPDFHandler> _logger;
    // Inject only what THIS feature needs!

    public ExportToPDFHandler(
        ApplicationDbContext context,
        ILogger<ExportToPDFHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<byte[]>> Handle(
        ExportToPDFCommand request,
        CancellationToken cancellationToken)
    {
        // All PDF export logic HERE
        // No PdfService! Just do it here.
        // It's ok to have 100+ lines in a handler for complex features.
        
        var application = await _context.IRRRLApplications
            .Include(a => a.Borrower)
            .Include(a => a.Property)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (application == null)
            return Result.Failure<byte[]>("Application not found");

        // Generate PDF...
        byte[] pdfBytes = GeneratePDF(application);
        
        return Result.Success(pdfBytes);
    }

    private byte[] GeneratePDF(IRRRLApplication application)
    {
        // PDF generation logic
        // Can be a private method in same file
        // Or extract to a nested class if it gets huge
        return Array.Empty<byte>();
    }
}
```

**Step 3: Use in Blazor Page**
```csharp
@inject IMediator Mediator

private async Task ExportToPDF()
{
    var result = await Mediator.Send(new ExportToPDFCommand(applicationId));
    
    if (result.IsSuccess)
    {
        // Download PDF
        await DownloadFile(result.Value);
    }
    else
    {
        // Show error
        errorMessage = result.Error;
    }
}
```

---

## 🔍 Real Examples in This Project

### 1. Loan Officer Dashboard

**File:** `Features/LoanOfficer/GetDashboard/GetDashboardQuery.cs`

```csharp
// Usage in Dashboard.razor:
var result = await Mediator.Send(new GetDashboardQuery());
allApplications = result.Applications;
stats = result.Statistics;
```

**What it does:**
- Loads applications from database
- Calculates statistics (New, In Progress, Ready for Review, Completed)
- Projects to lightweight DTOs
- Returns everything the dashboard needs

**Key point:** All logic in ONE file. No service layer!

### 2. Add Note to Application

**File:** `Features/LoanOfficer/AddNote/AddNoteCommand.cs`

```csharp
// Usage in NotesTab.razor:
var result = await Mediator.Send(new AddNoteCommand(
    ApplicationId: applicationId,
    Content: noteContent,
    NoteType: ApplicationNoteType.General,
    IsImportant: false,
    CreatedByUserId: currentUserId,
    CreatedByName: currentUserName
));

if (result.IsSuccess)
{
    // Refresh notes list
}
```

**What it does:**
- Validates note content
- Verifies application exists
- Creates note entity
- Saves to database
- Returns Result (success or failure)

**Key point:** Command pattern with validation. Returns `Result<T>` instead of throwing exceptions.

### 3. Underwriter Queue (Different Role = Different Feature)

**File:** `Features/Underwriter/GetQueue/GetQueueQuery.cs`

```csharp
// Completely different from LoanOfficer dashboard!
var result = await Mediator.Send(new GetQueueQuery(underwriterId));
```

**What it does:**
- Shows applications ready for underwriting (different status filter)
- Returns DIFFERENT data (priority, days in queue, document status)
- Uses DIFFERENT business logic (FIFO queue, priority calculation)
- Zero code sharing with LoanOfficer features

**Key point:** Different roles see different data, have different workflows. Vertical Slice makes this natural!

---

## 🎓 Comparison: Old Service Layer vs Vertical Slice

### Old Way (Service Layer)
```csharp
// ILoanOfficerService.cs
public interface ILoanOfficerService
{
    Task<IEnumerable<IRRRLApplication>> GetAllApplicationsAsync(...);
    Task<IRRRLApplication?> GetApplicationDetailAsync(int id);
    Task<ApplicationStatistics> GetStatisticsAsync(...);
    Task UpdateApplicationStatusAsync(...);
    Task<IEnumerable<Document>> GetApplicationDocumentsAsync(...);
    Task<IEnumerable<ActionItem>> GetApplicationActionItemsAsync(...);
    Task<IEnumerable<ApplicationNote>> GetApplicationNotesAsync(...);
    Task<ApplicationNote?> AddNoteAsync(...);
    // 20 more methods...
}

// LoanOfficerService.cs - 500+ lines!
public class LoanOfficerService : ILoanOfficerService
{
    // Implements ALL methods
    // Even if you only need ONE method, you see all 20+
}

// Dashboard.razor
@inject ILoanOfficerService LoanOfficerService // Depends on ENTIRE service

var apps = await LoanOfficerService.GetAllApplicationsAsync();
```

**Problems:**
- God service with too many responsibilities
- Interface with 20+ methods
- Hard to test (mock entire interface)
- Tight coupling (change one method signature = recompile everything)

### New Way (Vertical Slice)
```csharp
// Features/LoanOfficer/GetDashboard/GetDashboardQuery.cs
public record GetDashboardQuery(...) : IQuery<GetDashboardResult>;
public class GetDashboardHandler { /* Only dashboard logic */ }

// Features/LoanOfficer/AddNote/AddNoteCommand.cs
public record AddNoteCommand(...) : ICommand<Result<ApplicationNote>>;
public class AddNoteHandler { /* Only add note logic */ }

// Dashboard.razor
@inject IMediator Mediator // Depends on MediatR (abstraction)

var result = await Mediator.Send(new GetDashboardQuery());
```

**Benefits:**
- Small, focused classes (Single Responsibility Principle)
- Easy to test (mock one handler)
- Loose coupling (add new features without changing existing code)
- Clear intent (query vs command)

---

## 🛠️ Common Patterns

### Pattern 1: Query with Optional Filter
```csharp
public record GetDashboardQuery(
    string? LoanOfficerId = null, // Optional filter
    string? SearchText = null,
    ApplicationStatus? StatusFilter = null
) : IQuery<GetDashboardResult>;
```

### Pattern 2: Command with Result Pattern
```csharp
public record AddNoteCommand(...) : ICommand<Result<ApplicationNote>>;

// Handler returns:
return Result.Success(note); // Or
return Result.Failure<ApplicationNote>("Error message");

// Caller checks:
if (result.IsSuccess)
{
    var note = result.Value;
}
else
{
    ShowError(result.Error);
}
```

### Pattern 3: Command without Return Value
```csharp
public record UpdateStatusCommand(...) : ICommand<Result>;

// Handler returns:
return Result.Success(); // Or
return Result.Failure("Error message");
```

### Pattern 4: Nested DTOs in Result
```csharp
public record GetDashboardResult(
    List<ApplicationSummaryDto> Applications, // List of DTOs
    DashboardStatistics Statistics // Nested DTO
);

public record DashboardStatistics(
    int NewCount,
    int InProgressCount,
    // ...
);
```

---

## 📦 Project Structure

```
IRRRL.Web/
├── Features/
│   ├── Common/                          # Shared ONLY when truly needed
│   │   ├── IQuery.cs                    # Marker interface
│   │   ├── ICommand.cs                  # Marker interface
│   │   ├── Result.cs                    # Result pattern
│   │   └── Behaviors/
│   │       └── LoggingBehavior.cs       # MediatR pipeline (logs all requests)
│   │
│   ├── LoanOfficer/                     # Loan Officer features
│   │   ├── GetDashboard/
│   │   │   └── GetDashboardQuery.cs     # Query + Handler + DTOs
│   │   ├── GetApplicationDetail/
│   │   │   └── GetApplicationDetailQuery.cs
│   │   ├── AddNote/
│   │   │   └── AddNoteCommand.cs
│   │   └── UpdateStatus/
│   │       └── UpdateStatusCommand.cs
│   │
│   ├── Veteran/                         # Veteran features
│   │   └── SubmitApplication/
│   │       └── SubmitApplicationCommand.cs
│   │
│   ├── Underwriter/                     # Underwriter features
│   │   └── GetQueue/
│   │       └── GetQueueQuery.cs
│   │
│   └── Processor/                       # Processor features (future)
│       └── (To be implemented)
│
├── Pages/                                # Blazor UI
│   ├── LoanOfficer/
│   │   ├── Dashboard.razor              # Uses GetDashboardQuery
│   │   └── ApplicationDetail.razor      # Uses GetApplicationDetailQuery, AddNoteCommand
│   └── Veteran/
│       └── Apply.razor                   # Uses SubmitApplicationCommand
│
└── Program.cs                            # MediatR registration
```

---

## 🧪 Testing Vertical Slices

### Unit Test Example
```csharp
public class GetDashboardHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsApplications_WhenApplicationsExist()
    {
        // Arrange
        var context = CreateInMemoryContext();
        await SeedTestData(context);
        var handler = new GetDashboardHandler(context, NullLogger<GetDashboardHandler>.Instance);
        var query = new GetDashboardQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Applications.Should().HaveCount(3);
        result.Statistics.NewCount.Should().Be(1);
    }
}
```

**Benefits:**
- Test ONE feature in isolation
- No need to mock huge service interfaces
- Fast tests (no unnecessary dependencies)

---

## 🎯 When to Share Code

**Rule:** Only share when absolutely necessary!

### ❌ Don't Share View Models
```csharp
// BAD: Shared/ApplicationDto.cs
public class ApplicationDto // Used by Veteran, LoanOfficer, Underwriter
{
    // 50 properties
}
```

### ✅ Do Share Domain Entities
```csharp
// GOOD: IRRRL.Core/Entities/IRRRLApplication.cs
public class IRRRLApplication // Database entity
{
    // This is the source of truth
}
```

### ✅ Do Share Common Utilities
```csharp
// GOOD: Features/Common/Result.cs
public class Result { } // Shared result pattern

// GOOD: Features/Common/Behaviors/LoggingBehavior.cs
public class LoggingBehavior { } // MediatR pipeline behavior
```

---

## 🚀 Migration Path

We're gradually moving from service layer to vertical slices:

### ✅ Migrated to Vertical Slice
- [x] LoanOfficer/GetDashboard
- [x] LoanOfficer/GetApplicationDetail
- [x] LoanOfficer/AddNote
- [x] LoanOfficer/UpdateStatus
- [x] Veteran/SubmitApplication (example)
- [x] Underwriter/GetQueue (example)

### 🔜 To Be Migrated
- [ ] Document upload
- [ ] Action item management
- [ ] NTB calculator
- [ ] PDF generation
- [ ] Veteran application form (multi-step)

### 📋 Old Services (To Be Removed)
- [ ] `ILoanOfficerService` → Delete after all features migrated
- [ ] `LoanOfficerService` → Delete after all features migrated

---

## 📚 Further Reading

- **Jimmy Bogard** - [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)
- **CodeOpinion YouTube** - [Vertical Slice Architecture Playlist](https://www.youtube.com/c/CodeOpinion)
- **MediatR** - [GitHub Repository](https://github.com/jbogard/MediatR)
- **Clean Architecture** - Still relevant for domain entities and infrastructure concerns

---

## 💡 Key Takeaways

1. **Feature = One Folder** - Everything for a feature lives together
2. **No Shared View Models** - Each feature defines its own DTOs
3. **CQRS** - Separate Queries (read) from Commands (write)
4. **Result Pattern** - Return `Result<T>` instead of throwing exceptions
5. **MediatR** - Send queries/commands through MediatR pipeline
6. **Independent Roles** - Veteran, LoanOfficer, Underwriter, Processor features don't share code
7. **Easy to Delete** - Remove a feature = Delete one folder
8. **Easy to Find** - "Where's Add Note?" → `Features/LoanOfficer/AddNote/`

---

## ❓ FAQ

**Q: Isn't this code duplication?**  
A: Some duplication is OK! "Duplication is far cheaper than the wrong abstraction" - Sandi Metz. Each feature can evolve independently.

**Q: What about shared business logic?**  
A: Put it in `IRRRL.Core/Services/` (like `NetTangibleBenefitCalculator`, `EligibilityService`). These are true domain services.

**Q: Do I need a repository layer?**  
A: No! Inject `ApplicationDbContext` directly into handlers. Entity Framework IS your repository.

**Q: What if two features need the same data?**  
A: Each feature queries the database independently. Database is optimized for reads. Trust EF Core caching.

**Q: How do I share validation logic?**  
A: Create a FluentValidation validator in the feature folder. If truly shared, put in `Features/Common/Validators/`.

---

Happy slicing! 🍕


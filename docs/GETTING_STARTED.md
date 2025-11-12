# Getting Started with IRRRL Processing System

## Prerequisites

Before you begin, ensure you have the following installed:

1. **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **SQL Server** - LocalDB, Express, or Full version
3. **Visual Studio 2022** or **Visual Studio Code** with C# extension
4. **Git** (optional, for version control)

## Installation Steps

### 1. Verify .NET Installation

Open a terminal/command prompt and run:

```bash
dotnet --version
```

You should see version 8.0.x or higher.

### 2. Clone or Navigate to the Project

```bash
cd "C:\Mac\Home\Desktop\Repos\IRRRL AI"
```

### 3. Restore NuGet Packages

```bash
dotnet restore
```

This will download all required dependencies for all projects in the solution.

### 4. Configure Database Connection

Edit `IRRRL.Web/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IRRRLDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**For SQL Server Express**, use:
```
Server=.\\SQLEXPRESS;Database=IRRRLDb;Trusted_Connection=true;MultipleActiveResultSets=true
```

**For Full SQL Server**, use:
```
Server=localhost;Database=IRRRLDb;Trusted_Connection=true;MultipleActiveResultSets=true
```

### 5. Create Database and Run Migrations

```bash
dotnet ef database update --project IRRRL.Infrastructure --startup-project IRRRL.Web
```

This will:
- Create the database
- Create all tables
- Set up Identity tables
- Seed default roles
- Create default admin user

### 6. (Optional) Configure Azure OpenAI

If you want to enable AI features, edit `IRRRL.Web/appsettings.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "YOUR_API_KEY_HERE",
    "ModelName": "gpt-4",
    "MaxTokens": 2000,
    "Temperature": 0.7
  }
}
```

**Note**: The application will work without AI configuration, but AI features will be disabled.

### 7. Build the Solution

```bash
dotnet build
```

Verify that all projects build successfully with no errors.

### 8. Run the Application

```bash
dotnet run --project IRRRL.Web
```

The application should start and display URLs like:
```
Now listening on: https://localhost:7001
Now listening on: http://localhost:5000
```

### 9. Access the Application

Open your browser and navigate to:
```
https://localhost:7001
```

## Default Login Credentials

The system creates a default administrator account:

- **Email**: `admin@irrrl.local`
- **Password**: `Admin@123!`

**⚠️ IMPORTANT**: Change this password immediately after first login in production!

## Creating Test Users

### Option 1: Using the Application
1. Log in as admin
2. Navigate to Admin → Manage Users
3. Create new users with appropriate roles

### Option 2: Using Database Seeder
The `TestDataSeeder` class can create sample data. To use it:

1. Edit `IRRRL.Web/Program.cs` and add after database initialization:

```csharp
// Seed test data (Development only)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await TestDataSeeder.SeedTestDataAsync(context);
    }
}
```

2. Restart the application

This will create:
- Sample veteran borrower
- Sample property
- Sample IRRRL application
- Sample action items
- Complete test workflow

## User Roles

The system has four user roles:

1. **Veteran**
   - Submit new applications
   - Track application status
   - Upload documents

2. **LoanOfficer**
   - View all applications
   - Gather documents (guided by AI action items)
   - Review and approve cash-out applications

3. **Underwriter**
   - Review completed application packages
   - Approve or decline applications

4. **Administrator**
   - All privileges
   - User management
   - System configuration

## Testing the Application

### Run Unit Tests

```bash
dotnet test
```

This will run all tests in the `IRRRL.Tests` project.

### Test Specific Features

1. **Net Tangible Benefit Calculator**:
   - Unit tests in `NetTangibleBenefitCalculatorTests.cs`
   - Tests various scenarios including edge cases

2. **Eligibility Service**:
   - Unit tests in `EligibilityServiceTests.cs`
   - Validates VA IRRRL requirements

## Common Issues and Solutions

### Issue: Database Connection Failed

**Solution**: 
- Verify SQL Server is running
- Check connection string is correct
- Ensure database exists (run migrations again)

### Issue: Package Restore Failed

**Solution**:
```bash
dotnet nuget locals all --clear
dotnet restore --force
```

### Issue: Migration Failed

**Solution**:
```bash
# Drop database and recreate
dotnet ef database drop --project IRRRL.Infrastructure --startup-project IRRRL.Web --force
dotnet ef database update --project IRRRL.Infrastructure --startup-project IRRRL.Web
```

### Issue: Port Already in Use

**Solution**: 
Edit `IRRRL.Web/Properties/launchSettings.json` and change the port numbers.

### Issue: SignalR Connection Failed

**Solution**:
- Check browser console for errors
- Ensure JavaScript is enabled
- Verify the SignalR hub URL in `signalr-client.js`

## Project Structure

```
IRRRL AI/
├── IRRRL.Web/              # Blazor Server application
│   ├── Pages/              # Razor pages
│   ├── Components/         # Reusable Blazor components
│   ├── Hubs/               # SignalR hubs
│   ├── wwwroot/            # Static files (CSS, JS)
│   └── Program.cs          # Application entry point
│
├── IRRRL.Core/             # Business logic
│   ├── Entities/           # Domain models
│   ├── Enums/              # Enumerations
│   ├── Services/           # Business services
│   └── Interfaces/         # Service interfaces
│
├── IRRRL.Infrastructure/   # Data and external services
│   ├── Data/               # EF Core DbContext
│   ├── AI/                 # AI service implementations
│   ├── Documents/          # PDF generation
│   └── Services/           # Infrastructure services
│
├── IRRRL.Shared/           # Shared code
│   ├── DTOs/               # Data transfer objects
│   └── Constants/          # Application constants
│
└── IRRRL.Tests/            # Tests
    ├── Unit/               # Unit tests
    └── Integration/        # Integration tests
```

## Next Steps

1. **Explore the Code**:
   - Start with domain models in `IRRRL.Core/Entities`
   - Review business logic in `IRRRL.Core/Services`
   - Check out the workflow in `ApplicationWorkflowService`

2. **Customize for Your Needs**:
   - Update VA requirements in `ApplicationConstants`
   - Modify document templates in `QuestPDFDocumentGenerator`
   - Customize UI components in `IRRRL.Web`

3. **Add Missing Pages**:
   - Veteran application form
   - Loan officer dashboard
   - Document upload interface
   - Application review screens

4. **Configure Production Environment**:
   - Set up production database
   - Configure Azure OpenAI (optional)
   - Set up SSL certificates
   - Configure monitoring and logging

## Support and Documentation

- **Code Documentation**: XML comments throughout codebase
- **Architecture**: See `IMPLEMENTATION_SUMMARY.md`
- **README**: See `README.md` for project overview

## Development Workflow

1. **Make Changes**: Edit code in your preferred IDE
2. **Build**: `dotnet build`
3. **Test**: `dotnet test`
4. **Run**: `dotnet run --project IRRRL.Web`
5. **Debug**: Use Visual Studio debugger or VS Code

## Deployment

### To IIS:
```bash
dotnet publish -c Release -o ./publish
```
Then deploy the `publish` folder to IIS.

### To Azure:
Use Visual Studio's Azure publish feature or Azure DevOps pipelines.

## Tips for Development

1. **Use Hot Reload**: ASP.NET Core supports hot reload for rapid development
2. **Enable Detailed Errors**: Set `DetailedErrors: true` in Development
3. **Check Logs**: Logs are written to `Logs/` directory
4. **Use Database Seeder**: Populate test data for development
5. **Test Real-time Updates**: Open multiple browser windows to test SignalR

## Security Checklist

Before deploying to production:

- [ ] Change default admin password
- [ ] Update connection strings
- [ ] Set strong password policies
- [ ] Enable HTTPS only
- [ ] Configure CORS appropriately
- [ ] Set up API key management
- [ ] Enable audit logging
- [ ] Configure file upload limits
- [ ] Set up database backups
- [ ] Review error handling

---

**Questions or Issues?**

Check the code comments and XML documentation for detailed information about each component.


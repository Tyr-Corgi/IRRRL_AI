# IRRRL VA Loan Processing Application

An ASP.NET Core Blazor Server application for processing VA IRRRL (Interest Rate Reduction Refinance Loan) applications with AI-driven automation.

## Overview

This system provides a streamlined workflow for processing VA IRRRL applications with:
- **Veteran Portal**: Simple data entry with instant savings calculations
- **Loan Officer Dashboard**: AI-generated action items for efficient document gathering
- **AI Processing**: Automated eligibility verification, document validation, and form generation
- **Real-time Updates**: SignalR-powered status tracking for all users

## Project Structure

```
IRRRL.Web/              # Blazor Server application
IRRRL.Core/             # Business logic and domain models
IRRRL.Infrastructure/   # Data access and external services
IRRRL.Shared/           # Shared DTOs and constants
IRRRL.Tests/            # Unit and integration tests
```

## Key Features

### Rate-and-Term Refinance (Primary Focus)
- Minimal documentation required
- No income verification needed
- No appraisal required
- Fast AI-driven processing
- Automatic eligibility validation

### Net Tangible Benefit Calculator
- Monthly payment comparison (P&I only)
- Interest rate reduction validation (≥0.5% for fixed-to-fixed)
- Recoupment period calculation (must be ≤36 months)
- Total savings projections
- Equity growth analysis

### AI-Powered Features
- Instant eligibility validation
- Automated document quality checking
- OCR and data extraction
- Dynamic action item generation for loan officers
- Compliance verification
- Automated form population

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Frontend**: Blazor Server
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Document Generation**: QuestPDF
- **AI Integration**: Azure OpenAI
- **Real-time**: SignalR
- **Logging**: Serilog
- **Testing**: xUnit, Moq, FluentAssertions

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or full version)
- Azure OpenAI API key (for AI features)

### Configuration

1. Update `appsettings.json` with your connection strings and API keys:
   - Database connection string
   - Azure OpenAI endpoint and API key
   - Document storage path

2. Run database migrations:
   ```bash
   dotnet ef database update --project IRRRL.Infrastructure --startup-project IRRRL.Web
   ```

3. Run the application:
   ```bash
   dotnet run --project IRRRL.Web
   ```

## User Roles

- **Veteran**: Submit applications and track status
- **LoanOfficer**: Gather documents using AI-generated checklists
- **Underwriter**: Review completed file packages

## Development Workflow

1. Create domain models in `IRRRL.Core`
2. Implement data access in `IRRRL.Infrastructure`
3. Build Blazor UI components in `IRRRL.Web`
4. Write tests in `IRRRL.Tests`

## License

Proprietary - All Rights Reserved


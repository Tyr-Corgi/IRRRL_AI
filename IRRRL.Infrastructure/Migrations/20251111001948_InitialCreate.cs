using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRRRL.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorrowerId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Borrowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SSN = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VAFileNumber = table.Column<string>(type: "TEXT", nullable: true),
                    HasDisabilityRating = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisabilityPercentage = table.Column<int>(type: "INTEGER", nullable: true),
                    StreetAddress = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    ZipCode = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StreetAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    ZipCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    County = table.Column<string>(type: "TEXT", nullable: true),
                    PropertyType = table.Column<string>(type: "TEXT", nullable: false),
                    YearBuilt = table.Column<int>(type: "INTEGER", nullable: true),
                    SquareFeet = table.Column<int>(type: "INTEGER", nullable: true),
                    CurrentlyOccupied = table.Column<bool>(type: "INTEGER", nullable: false),
                    PreviouslyOccupied = table.Column<bool>(type: "INTEGER", nullable: false),
                    OccupancyStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OccupancyEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    AnnualPropertyTax = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    AnnualInsurance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IRRRLApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ApplicationType = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    BorrowerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedLoanAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RequestedInterestRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 3, nullable: false),
                    RequestedTermMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    NewLoanType = table.Column<int>(type: "INTEGER", nullable: false),
                    CashOutAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    CashOutPurpose = table.Column<string>(type: "TEXT", nullable: true),
                    EstimatedNewMonthlyPayment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    EstimatedMonthlySavings = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RecoupmentPeriodMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    MeetsNetTangibleBenefit = table.Column<bool>(type: "INTEGER", nullable: false),
                    FundingFeePercentage = table.Column<decimal>(type: "TEXT", precision: 5, scale: 3, nullable: false),
                    FundingFeeAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    FundingFeeWaived = table.Column<bool>(type: "INTEGER", nullable: false),
                    TotalClosingCosts = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalLoanCosts = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    EligibilityVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    EligibilityNotes = table.Column<string>(type: "TEXT", nullable: true),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApprovedBy = table.Column<string>(type: "TEXT", nullable: true),
                    DeclineReason = table.Column<string>(type: "TEXT", nullable: true),
                    AssignedLoanOfficerId = table.Column<string>(type: "TEXT", nullable: true),
                    AssignedUnderwriterId = table.Column<string>(type: "TEXT", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimatedClosingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualClosingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRRRLApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRRRLApplications_Borrowers_BorrowerId",
                        column: x => x.BorrowerId,
                        principalTable: "Borrowers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRRRLApplications_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IRRRLApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RelatedDocumentType = table.Column<int>(type: "INTEGER", nullable: true),
                    RelatedDocumentId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssignedToUserId = table.Column<string>(type: "TEXT", nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CompletionNotes = table.Column<string>(type: "TEXT", nullable: true),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    DependsOnActionItemIds = table.Column<string>(type: "TEXT", nullable: true),
                    GeneratedByAI = table.Column<bool>(type: "INTEGER", nullable: false),
                    AIReasoning = table.Column<string>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimatedMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionItems_IRRRLApplications_IRRRLApplicationId",
                        column: x => x.IRRRLApplicationId,
                        principalTable: "IRRRLApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IRRRLApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ToStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangedByUserId = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationStatusHistories_IRRRLApplications_IRRRLApplicationId",
                        column: x => x.IRRRLApplicationId,
                        principalTable: "IRRRLApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IRRRLApplicationId = table.Column<int>(type: "INTEGER", nullable: true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    UserRole = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: true),
                    IsAIAction = table.Column<bool>(type: "INTEGER", nullable: false),
                    AIModel = table.Column<string>(type: "TEXT", nullable: true),
                    AIPrompt = table.Column<string>(type: "TEXT", nullable: true),
                    AIResponse = table.Column<string>(type: "TEXT", nullable: true),
                    OldValues = table.Column<string>(type: "TEXT", nullable: true),
                    NewValues = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_IRRRLApplications_IRRRLApplicationId",
                        column: x => x.IRRRLApplicationId,
                        principalTable: "IRRRLApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrentLoans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoanNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Lender = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LoanType = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    OriginalLoanAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    InterestRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 3, nullable: false),
                    RemainingTermMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalTermMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MonthlyPrincipalAndInterest = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MonthlyPropertyTax = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MonthlyInsurance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MonthlyPMI = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalMonthlyPayment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CurrentOnPayments = table.Column<bool>(type: "INTEGER", nullable: false),
                    LatePaymentsLast12Months = table.Column<int>(type: "INTEGER", nullable: false),
                    LatePaymentsOver30Days = table.Column<int>(type: "INTEGER", nullable: false),
                    LastLatePaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsVALoan = table.Column<bool>(type: "INTEGER", nullable: false),
                    OriginalVAFundingFee = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IRRRLApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentLoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrentLoans_IRRRLApplications_IRRRLApplicationId",
                        column: x => x.IRRRLApplicationId,
                        principalTable: "IRRRLApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IRRRLApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    DocumentType = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    IsGenerated = table.Column<bool>(type: "INTEGER", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsValidated = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValidatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ValidationNotes = table.Column<string>(type: "TEXT", nullable: true),
                    AIProcessed = table.Column<bool>(type: "INTEGER", nullable: false),
                    AIProcessedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExtractedData = table.Column<string>(type: "TEXT", nullable: true),
                    AIConfidenceScore = table.Column<decimal>(type: "TEXT", precision: 3, scale: 2, nullable: true),
                    AIProcessingNotes = table.Column<string>(type: "TEXT", nullable: true),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsLegible = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    QualityIssues = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousVersionId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsCurrentVersion = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_IRRRLApplications_IRRRLApplicationId",
                        column: x => x.IRRRLApplicationId,
                        principalTable: "IRRRLApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NetTangibleBenefits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IRRRLApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentInterestRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 3, nullable: false),
                    CurrentMonthlyPayment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CurrentRemainingTermMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    NewInterestRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 3, nullable: false),
                    NewMonthlyPayment = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    NewTermMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    InterestRateReduction = table.Column<decimal>(type: "TEXT", precision: 5, scale: 3, nullable: false),
                    MonthlyPaymentSavings = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalLoanCosts = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RecoupmentPeriodMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalSavingsOverRemainingTerm = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalInterestSavings = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TermReductionMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    EquityGrowthAcceleration = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    MeetsRecoupmentRequirement = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeetsInterestRateRequirement = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeetsPaymentReductionRequirement = table.Column<bool>(type: "INTEGER", nullable: false),
                    PassesNTBTest = table.Column<bool>(type: "INTEGER", nullable: false),
                    BreakEvenMonths = table.Column<decimal>(type: "TEXT", precision: 8, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CalculatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CalculatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetTangibleBenefits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetTangibleBenefits_IRRRLApplications_IRRRLApplicationId",
                        column: x => x.IRRRLApplicationId,
                        principalTable: "IRRRLApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionItems_AssignedToUserId",
                table: "ActionItems",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionItems_IRRRLApplicationId",
                table: "ActionItems",
                column: "IRRRLApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionItems_Priority",
                table: "ActionItems",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ActionItems_Status",
                table: "ActionItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_ChangedAt",
                table: "ApplicationStatusHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_IRRRLApplicationId",
                table: "ApplicationStatusHistories",
                column: "IRRRLApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_IRRRLApplicationId",
                table: "AuditLogs",
                column: "IRRRLApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_IsAIAction",
                table: "AuditLogs",
                column: "IsAIAction");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowers_Email",
                table: "Borrowers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowers_UserId",
                table: "Borrowers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrentLoans_IRRRLApplicationId",
                table: "CurrentLoans",
                column: "IRRRLApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentType",
                table: "Documents",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IRRRLApplicationId",
                table: "Documents",
                column: "IRRRLApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IsCurrentVersion",
                table: "Documents",
                column: "IsCurrentVersion");

            migrationBuilder.CreateIndex(
                name: "IX_IRRRLApplications_ApplicationNumber",
                table: "IRRRLApplications",
                column: "ApplicationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IRRRLApplications_ApplicationType",
                table: "IRRRLApplications",
                column: "ApplicationType");

            migrationBuilder.CreateIndex(
                name: "IX_IRRRLApplications_BorrowerId",
                table: "IRRRLApplications",
                column: "BorrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_IRRRLApplications_PropertyId",
                table: "IRRRLApplications",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_IRRRLApplications_Status",
                table: "IRRRLApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NetTangibleBenefits_IRRRLApplicationId",
                table: "NetTangibleBenefits",
                column: "IRRRLApplicationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionItems");

            migrationBuilder.DropTable(
                name: "ApplicationStatusHistories");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CurrentLoans");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "NetTangibleBenefits");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "IRRRLApplications");

            migrationBuilder.DropTable(
                name: "Borrowers");

            migrationBuilder.DropTable(
                name: "Properties");
        }
    }
}

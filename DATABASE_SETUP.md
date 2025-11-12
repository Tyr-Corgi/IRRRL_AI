# 🗄️ Database Setup Guide

## Quick Start

### Option 1: Automatic Reset (Recommended for Development)

Run the reset script from the `IRRRL.Web` directory:

```cmd
cd IRRRL.Web
reset-database.cmd
```

This will:
1. ✅ Delete the existing database
2. ✅ Delete all existing migrations  
3. ✅ Create a fresh `InitialCreate` migration
4. ✅ Prompt you to run the application

Then simply run:
```cmd
dotnet run --project IRRRL.Web
```

The application will automatically:
- Apply the migration
- Create all tables
- Seed test data

---

### Option 2: Manual Steps

#### 1. Delete Existing Database
```cmd
cd IRRRL.Web
del irrrl.db
del irrrl.db-shm
del irrrl.db-wal
```

#### 2. Delete Existing Migrations
```cmd
cd ..
rmdir /s /q IRRRL.Infrastructure\Migrations
```

#### 3. Create New Migration
```cmd
dotnet ef migrations add InitialCreate --project IRRRL.Infrastructure --startup-project IRRRL.Web
```

#### 4. Run Application (applies migration & seeds data)
```cmd
dotnet run --project IRRRL.Web
```

---

## 🔐 Test Accounts

After seeding, you can log in with these accounts:

### Administrator
- **Email**: `admin@irrrl.local`
- **Password**: `Admin@123!`
- **Role**: Administrator

### Loan Officer
- **Email**: `loanofficer@irrrl.local`
- **Password**: `LoanOfficer@123!`
- **Role**: Loan Officer
- **Name**: Sarah Officer

### Veteran
- **Email**: `veteran@irrrl.local`
- **Password**: `Veteran@123!`
- **Role**: Veteran
- **Name**: John Veteran

---

## 📊 Seeded Test Data

### 1 Complete Application
- **Application Number**: IRRRL-2025-001
- **Status**: Submitted (New)
- **Borrower**: John M. Smith
- **Property**: 123 Main St, San Diego, CA 92101
- **Current Loan**: $250,000 @ 4.5%
- **Proposed Rate**: 3.5%
- **Monthly Savings**: $377
- **Assigned To**: Sarah Officer (Loan Officer)

### Included Data:
- ✅ Complete borrower profile
- ✅ Property information
- ✅ Current loan details
- ✅ 3 Action items (2 pending, 1 completed)
- ✅ 2 Notes (1 contact log, 1 calculation)
- ✅ Net Tangible Benefit calculations

---

## 🔍 Verify Data

### Check Users
```sql
SELECT Email, UserName FROM AspNetUsers;
```

### Check Applications
```sql
SELECT ApplicationNumber, Status, SubmittedDate 
FROM IRRRLApplications;
```

### Check Action Items
```sql
SELECT Title, Priority, Status 
FROM ActionItems;
```

### Check Notes
```sql
SELECT NoteType, Content, CreatedByName 
FROM ApplicationNotes;
```

---

## 🚀 Using the Loan Officer Dashboard

1. **Log in** as Loan Officer:
   - Navigate to: `http://localhost:5000/Account/Login`
   - Email: `loanofficer@irrrl.local`
   - Password: `LoanOfficer@123!`

2. **View Dashboard**:
   - Automatically redirected to `/loanofficer/dashboard`
   - See 1 new application
   - View statistics (1 New, 0 In Progress, 0 Ready, 0 Completed)

3. **Review Application**:
   - Click "Review" on IRRRL-2025-001
   - Explore all 5 tabs:
     - **Overview**: Complete borrower & loan info
     - **Documents**: 6-item checklist (AI-generated possible)
     - **Action Items**: 3 tasks (2 pending, 1 done)
     - **NTB Calculator**: Pre-populated with loan data
     - **Notes**: 2 existing notes

4. **Test Features**:
   - Add a new note
   - Mark action items complete
   - Calculate NTB with different rates
   - Update application status

---

## 🛠️ Troubleshooting

### "Database is locked"
- Close all connections to the database
- Restart the application
- Delete `.db-shm` and `.db-wal` files

### "Migration already exists"
- Delete the `Migrations` folder
- Run the reset script again

### "No applications showing"
- Check that seed data ran (look for console logs on startup)
- Verify logged in as Loan Officer
- Check database with SQL query

### "Build errors"
- Clean and rebuild solution:
  ```cmd
  dotnet clean
  dotnet build
  ```

---

## 📝 Notes

- **SQLite** is used for development (easy setup, no installation)
- **Production** should use SQL Server or PostgreSQL
- **Encryption keys** are stored in `%LOCALAPPDATA%\ASP.NET\DataProtection-Keys`
- **SSN encryption** happens automatically via `IDataProtectionService`
- **Migrations** are in `IRRRL.Infrastructure/Migrations`

---

## ⚠️ Important

**DO NOT commit** `irrrl.db` to git (already in `.gitignore`)  
**DO NOT commit** encryption keys to git  
**CHANGE** default passwords in production  
**BACKUP** encryption keys for production deployments  

---

**Ready to test?** Run `reset-database.cmd` and then `dotnet run --project IRRRL.Web`! 🎉


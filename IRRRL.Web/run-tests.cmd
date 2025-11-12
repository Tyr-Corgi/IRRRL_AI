@echo off
REM IRRRL Application Testing Script
REM Run this from the project root directory

echo ========================================
echo IRRRL Application Test Suite
echo ========================================
echo.

cd /d "%~dp0\.."

echo [1/5] Cleaning previous builds...
dotnet clean > nul 2>&1
echo     ✓ Clean complete

echo [2/5] Building solution...
dotnet build --configuration Debug
if %errorlevel% neq 0 (
    echo     ✗ Build FAILED!
    echo     Please fix compilation errors before testing.
    pause
    exit /b 1
)
echo     ✓ Build successful

echo [3/5] Running unit tests...
dotnet test IRRRL.Tests\IRRRL.Tests.csproj --no-build --verbosity quiet
if %errorlevel% neq 0 (
    echo     ⚠ Some tests failed - check output above
) else (
    echo     ✓ All tests passed
)

echo [4/5] Checking for linter errors...
echo     (Skipping - requires IDE integration)

echo [5/5] Database check...
if exist "IRRRL_Dev.db" (
    echo     ✓ Development database exists
) else (
    echo     ⚠ Database not found - will be created on first run
    echo     Run: dotnet ef database update --project IRRRL.Infrastructure --startup-project IRRRL.Web
)

echo.
echo ========================================
echo Test Summary
echo ========================================
echo Status: Ready for manual testing
echo.
echo Next Steps:
echo 1. Start the application: dotnet run --project IRRRL.Web
echo 2. Open browser: http://localhost:5000
echo 3. Login as: veteran@irrrl.local / Veteran@123!
echo 4. Follow testing guide: IRRRL.Web\TESTING_GUIDE.md
echo.
echo ========================================

pause


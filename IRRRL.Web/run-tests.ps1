# IRRRL Application Testing Script (PowerShell)
# Run this from the project root directory

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "IRRRL Application Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Change to project root
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location (Join-Path $scriptPath "..")

Write-Host "[1/5] Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean | Out-Null
Write-Host "     ✓ Clean complete" -ForegroundColor Green

Write-Host "[2/5] Building solution..." -ForegroundColor Yellow
$buildResult = dotnet build --configuration Debug 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "     ✗ Build FAILED!" -ForegroundColor Red
    Write-Host "     Please fix compilation errors before testing." -ForegroundColor Red
    Write-Host ""
    Write-Host $buildResult -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host "     ✓ Build successful" -ForegroundColor Green

Write-Host "[3/5] Running unit tests..." -ForegroundColor Yellow
$testResult = dotnet test IRRRL.Tests\IRRRL.Tests.csproj --no-build --verbosity quiet 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "     ⚠ Some tests failed - check output above" -ForegroundColor Yellow
    Write-Host $testResult
} else {
    Write-Host "     ✓ All tests passed" -ForegroundColor Green
}

Write-Host "[4/5] Checking for linter errors..." -ForegroundColor Yellow
Write-Host "     (Manual check required - see TESTING_GUIDE.md)" -ForegroundColor Gray

Write-Host "[5/5] Database check..." -ForegroundColor Yellow
if (Test-Path "IRRRL_Dev.db") {
    Write-Host "     ✓ Development database exists" -ForegroundColor Green
} else {
    Write-Host "     ⚠ Database not found - will be created on first run" -ForegroundColor Yellow
    Write-Host "     Run: dotnet ef database update --project IRRRL.Infrastructure --startup-project IRRRL.Web" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Status: " -NoNewline
Write-Host "Ready for manual testing" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Start the application: " -NoNewline
Write-Host "dotnet run --project IRRRL.Web" -ForegroundColor White
Write-Host "2. Open browser: " -NoNewline
Write-Host "http://localhost:5000" -ForegroundColor Cyan
Write-Host "3. Login as: " -NoNewline
Write-Host "veteran@irrrl.local / Veteran@123!" -ForegroundColor White
Write-Host "4. Follow testing guide: " -NoNewline
Write-Host "IRRRL.Web\TESTING_GUIDE.md" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

Read-Host "Press Enter to exit"


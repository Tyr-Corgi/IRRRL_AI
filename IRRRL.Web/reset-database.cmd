@echo off
echo Resetting IRRRL Database...
echo.

REM Delete existing database
if exist irrrl.db (
    echo Deleting existing database...
    del irrrl.db
    del irrrl.db-shm 2>nul
    del irrrl.db-wal 2>nul
    echo Database deleted.
    echo.
)

REM Delete existing migrations
if exist ..\IRRRL.Infrastructure\Migrations (
    echo Deleting existing migrations...
    rmdir /s /q ..\IRRRL.Infrastructure\Migrations
    echo Migrations deleted.
    echo.
)

REM Create new migration
echo Creating new migration...
cd ..
dotnet ef migrations add InitialCreate --project IRRRL.Infrastructure --startup-project IRRRL.Web
echo.

echo Migration created!
echo.
echo Run 'dotnet run --project IRRRL.Web' to apply migration and seed data.
pause


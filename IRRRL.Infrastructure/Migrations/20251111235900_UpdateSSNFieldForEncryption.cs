using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRRRL.Infrastructure.Migrations
{
    /// <summary>
    /// This migration updates the SSN field in the Borrowers table to support encrypted values.
    /// 
    /// WHY: Encrypted SSNs are much longer than the original 11-character format (123-45-6789).
    /// The Data Protection API produces base64-encoded strings that are typically 200+ characters.
    /// 
    /// WHAT IT DOES:
    /// 1. Changes the Borrowers.SSN column from MaxLength(11) to MaxLength(500)
    /// 2. This allows us to store encrypted SSN values safely
    /// 
    /// NOTE: Existing data will NOT be automatically encrypted. You'll need to:
    /// - Run a data migration script to encrypt existing SSNs, OR
    /// - Start fresh with a new database (for development only)
    /// </summary>
    public partial class UpdateSSNFieldForEncryption : Migration
    {
        /// <summary>
        /// Applies the migration - expands SSN column to 500 characters
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SSN",
                table: "Borrowers",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 11);
        }

        /// <summary>
        /// Rolls back the migration - returns SSN column to 11 characters
        /// WARNING: This will truncate any encrypted SSNs in the database!
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SSN",
                table: "Borrowers",
                type: "TEXT",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);
        }
    }
}


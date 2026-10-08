using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOA_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddFailedLoginAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only add the new column — the columns below were already dropped
            // by the Laravel simplify migration on the live database.
            migrationBuilder.AddColumn<int>(
                name: "failed_login_attempts",
                table: "members",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "failed_login_attempts",
                table: "members");
        }
    }
}

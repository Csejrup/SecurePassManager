using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurePassManager.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterPasswordHashedToCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MasterPasswordHashed",
                table: "Credentials",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterPasswordHashed",
                table: "Credentials");
        }
    }
}

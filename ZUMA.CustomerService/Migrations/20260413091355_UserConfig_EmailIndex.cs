using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.CustomerService.Migrations
{
    /// <inheritdoc />
    public partial class UserConfig_EmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Unique",
                schema: "public",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Unique",
                schema: "public",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "\"Deleted\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Unique",
                schema: "public",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Unique",
                schema: "public",
                table: "Users",
                column: "Email",
                unique: true);
        }
    }
}

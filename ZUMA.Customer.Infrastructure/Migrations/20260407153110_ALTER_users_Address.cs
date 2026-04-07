using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ALTER_users_Address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "public",
                table: "Users",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "public",
                table: "Users");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.BussinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class templates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmailTemplateType",
                schema: "dbo",
                table: "Emails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailTemplateType",
                schema: "dbo",
                table: "Emails");
        }
    }
}

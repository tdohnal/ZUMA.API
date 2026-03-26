using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.BussinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class emailsConfigure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Emails_RecipientId",
                schema: "dbo",
                table: "Emails");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_RecipientId",
                schema: "dbo",
                table: "Emails",
                column: "RecipientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Emails_RecipientId",
                schema: "dbo",
                table: "Emails");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_RecipientId",
                schema: "dbo",
                table: "Emails",
                column: "RecipientId",
                unique: true);
        }
    }
}

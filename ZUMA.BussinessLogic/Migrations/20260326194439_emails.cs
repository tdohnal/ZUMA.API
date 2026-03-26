using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.BussinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class emails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emails",
                schema: "dbo",
                columns: table => new
                {
                    InternalId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecipientId = table.Column<long>(type: "bigint", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sent = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.InternalId);
                    table.ForeignKey(
                        name: "FK_Emails_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "InternalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emails_Deleted",
                schema: "dbo",
                table: "Emails",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_RecipientId",
                schema: "dbo",
                table: "Emails",
                column: "RecipientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emails",
                schema: "dbo");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.CustomerService.Migrations
{
    /// <inheritdoc />
    public partial class controlElement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ControlElementId1",
                table: "ControlElementsItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ControlElementsItems_ControlElementId1",
                table: "ControlElementsItems",
                column: "ControlElementId1");

            migrationBuilder.CreateIndex(
                name: "IX_ControlElements_OwnerUserId",
                table: "ControlElements",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ControlElements_Users_OwnerUserId",
                table: "ControlElements",
                column: "OwnerUserId",
                principalSchema: "public",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ControlElementsItems_ControlElements_ControlElementId1",
                table: "ControlElementsItems",
                column: "ControlElementId1",
                principalTable: "ControlElements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ControlElements_Users_OwnerUserId",
                table: "ControlElements");

            migrationBuilder.DropForeignKey(
                name: "FK_ControlElementsItems_ControlElements_ControlElementId1",
                table: "ControlElementsItems");

            migrationBuilder.DropIndex(
                name: "IX_ControlElementsItems_ControlElementId1",
                table: "ControlElementsItems");

            migrationBuilder.DropIndex(
                name: "IX_ControlElements_OwnerUserId",
                table: "ControlElements");

            migrationBuilder.DropColumn(
                name: "ControlElementId1",
                table: "ControlElementsItems");
        }
    }
}

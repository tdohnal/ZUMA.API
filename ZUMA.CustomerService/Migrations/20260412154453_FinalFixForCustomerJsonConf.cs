using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.CustomerService.Migrations
{
    /// <inheritdoc />
    public partial class FinalFixForCustomerJsonConf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ControlElementsItems_UserControlElements_ControlElementId",
                table: "ControlElementsItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserControlElements",
                table: "UserControlElements");

            migrationBuilder.RenameTable(
                name: "UserControlElements",
                newName: "ControlElements");

            migrationBuilder.RenameIndex(
                name: "IX_UserControlElements_PublicId",
                table: "ControlElements",
                newName: "IX_ControlElements_PublicId");

            migrationBuilder.AlterColumn<long>(
                name: "ControlElementId",
                table: "ControlElementsItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ControlElements",
                table: "ControlElements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ControlElementsItems_ControlElements_ControlElementId",
                table: "ControlElementsItems",
                column: "ControlElementId",
                principalTable: "ControlElements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ControlElementsItems_ControlElements_ControlElementId",
                table: "ControlElementsItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ControlElements",
                table: "ControlElements");

            migrationBuilder.RenameTable(
                name: "ControlElements",
                newName: "UserControlElements");

            migrationBuilder.RenameIndex(
                name: "IX_ControlElements_PublicId",
                table: "UserControlElements",
                newName: "IX_UserControlElements_PublicId");

            migrationBuilder.AlterColumn<long>(
                name: "ControlElementId",
                table: "ControlElementsItems",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserControlElements",
                table: "UserControlElements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ControlElementsItems_UserControlElements_ControlElementId",
                table: "ControlElementsItems",
                column: "ControlElementId",
                principalTable: "UserControlElements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

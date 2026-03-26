using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.BussinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class Updateidentifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "Users",
                newName: "InternalId");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "Registrations",
                newName: "InternalId");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                schema: "dbo",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                schema: "dbo",
                table: "Registrations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PublicId",
                schema: "dbo",
                table: "Registrations");

            migrationBuilder.RenameColumn(
                name: "InternalId",
                schema: "dbo",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InternalId",
                schema: "dbo",
                table: "Registrations",
                newName: "Id");
        }
    }
}

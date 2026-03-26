using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.BussinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserName_Unique",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HashedPassword",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                schema: "dbo",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "AuthCode",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuthCodeExpiration",
                schema: "dbo",
                table: "Users",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthCode",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AuthCodeExpiration",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Token",
                schema: "dbo",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "HashedPassword",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                schema: "dbo",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName_Unique",
                schema: "dbo",
                table: "Users",
                column: "UserName",
                unique: true);
        }
    }
}

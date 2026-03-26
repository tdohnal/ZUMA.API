using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZUMA.BussinessLogic.Migrations
{
    /// <inheritdoc />
    public partial class refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                schema: "dbo",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AuthCodeExpiration",
                schema: "dbo",
                table: "Users",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "NULL",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AuthCodeExpiration",
                schema: "dbo",
                table: "Users",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "NULL");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}

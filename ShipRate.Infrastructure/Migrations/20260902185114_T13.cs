using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                schema: "cnt",
                table: "DetentionItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                schema: "sec",
                table: "Permissions",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "DisplayName", "IsActive", "Name" },
                values: new object[] { 151505, "Base Operations", new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Activate/Deactivate Agents", true, "BaseOperations.AgentsManagement.Activation" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "sec",
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 151505);

            migrationBuilder.DropColumn(
                name: "IsReturned",
                schema: "cnt",
                table: "DetentionItems");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmDate",
                schema: "ops",
                table: "ExportDocsReceipts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ConfirmUserId",
                schema: "ops",
                table: "ExportDocsReceipts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                schema: "ops",
                table: "ExportDocsReceipts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ExportDocsReceipts_ConfirmUserId",
                schema: "ops",
                table: "ExportDocsReceipts",
                column: "ConfirmUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportDocsReceipts_Users_ConfirmUserId",
                schema: "ops",
                table: "ExportDocsReceipts",
                column: "ConfirmUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportDocsReceipts_Users_ConfirmUserId",
                schema: "ops",
                table: "ExportDocsReceipts");

            migrationBuilder.DropIndex(
                name: "IX_ExportDocsReceipts_ConfirmUserId",
                schema: "ops",
                table: "ExportDocsReceipts");

            migrationBuilder.DropColumn(
                name: "ConfirmDate",
                schema: "ops",
                table: "ExportDocsReceipts");

            migrationBuilder.DropColumn(
                name: "ConfirmUserId",
                schema: "ops",
                table: "ExportDocsReceipts");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                schema: "ops",
                table: "ExportDocsReceipts");
        }
    }
}

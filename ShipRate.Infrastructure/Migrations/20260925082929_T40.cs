using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "base",
                table: "SystemObjectTypes",
                columns: new[] { "Id", "IsActive", "IsDeleted", "SystemObjectTypeLocalName", "SystemObjectTypeName" },
                values: new object[,]
                {
                    { 490, true, false, "درخواست وجه نقد از گاوصندوق", "CashPaymentRequests" },
                    { 491, true, false, "اقلام گاو صندوق", "VaultItems" },
                    { 492, true, false, "تراکنش گاوصندوق", "VaultMovements" },
                    { 493, true, false, "ورود و خروج اقلام گاو صندوق", "VaultItemCustodyLogs" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 490);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 491);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 492);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 493);
        }
    }
}

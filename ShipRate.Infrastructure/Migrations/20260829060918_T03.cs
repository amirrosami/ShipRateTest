using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BuyQuotes_RateRequestInquiryId",
                schema: "ops",
                table: "BuyQuotes");

            migrationBuilder.InsertData(
                schema: "base",
                table: "LegTypes",
                columns: new[] { "Id", "IsActive", "IsDeleted", "LegTypeName" },
                values: new object[,]
                {
                    { 5, true, false, "Pickup" },
                    { 6, true, false, "Delivery" }
                });

            migrationBuilder.InsertData(
                schema: "base",
                table: "SystemObjectTypes",
                columns: new[] { "Id", "IsActive", "IsDeleted", "SystemObjectTypeLocalName", "SystemObjectTypeName" },
                values: new object[] { 400, true, false, "رویدادها و لاگهای عملیاتی", "ActivityLogs" });

            migrationBuilder.CreateIndex(
                name: "IX_SellQuotes_SentByUserId",
                schema: "ops",
                table: "SellQuotes",
                column: "SentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyQuotes_RateRequestInquiryId_CarrierId_QuotationNo",
                schema: "ops",
                table: "BuyQuotes",
                columns: new[] { "RateRequestInquiryId", "CarrierId", "QuotationNo" },
                unique: true,
                filter: "[RateRequestInquiryId] IS NOT NULL AND [CarrierId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BLs_TranshipmentPortId",
                schema: "ops",
                table: "BLs",
                column: "TranshipmentPortId");

            migrationBuilder.AddForeignKey(
                name: "FK_BLs_Locations_TranshipmentPortId",
                schema: "ops",
                table: "BLs",
                column: "TranshipmentPortId",
                principalSchema: "base",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SellQuotes_Users_SentByUserId",
                schema: "ops",
                table: "SellQuotes",
                column: "SentByUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BLs_Locations_TranshipmentPortId",
                schema: "ops",
                table: "BLs");

            migrationBuilder.DropForeignKey(
                name: "FK_SellQuotes_Users_SentByUserId",
                schema: "ops",
                table: "SellQuotes");

            migrationBuilder.DropIndex(
                name: "IX_SellQuotes_SentByUserId",
                schema: "ops",
                table: "SellQuotes");

            migrationBuilder.DropIndex(
                name: "IX_BuyQuotes_RateRequestInquiryId_CarrierId_QuotationNo",
                schema: "ops",
                table: "BuyQuotes");

            migrationBuilder.DropIndex(
                name: "IX_BLs_TranshipmentPortId",
                schema: "ops",
                table: "BLs");

            migrationBuilder.DeleteData(
                schema: "base",
                table: "LegTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "LegTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 400);

            migrationBuilder.CreateIndex(
                name: "IX_BuyQuotes_RateRequestInquiryId",
                schema: "ops",
                table: "BuyQuotes",
                column: "RateRequestInquiryId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "base",
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentAttachmentFileName",
                schema: "ops",
                table: "DocumentAttachments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
                schema: "base",
                table: "ChargeTypes",
                columns: new[] { "Id", "AffectProfit", "AllowManualAmount", "AllowRateBased", "CalculationById", "CalculationTypeId", "ChargeTypeCategoryId", "ChargeTypeCode", "ChargeTypeCode2", "ChargeTypeLocalName", "ChargeTypeName", "CreateUserId", "CreatedAt", "CurrencyId", "DependsOnChargeTypeId", "EditUserId", "IsActive", "IsDeleted", "IsTaxable", "MultiplierRate", "Remarks", "SystemDefined", "UpdatedAt" },
                values: new object[,]
                {
                    { 108, null, null, null, 1, null, 2, "", "", "پلمپ", "Container Seal", null, null, null, null, null, true, false, true, 1m, "", true, null },
                    { 109, null, null, null, 1, null, 2, "", "", "حکم ترانشیپ", "Tranship Statement", null, null, null, null, null, true, false, true, 1m, "", true, null },
                    { 110, null, null, null, 1, null, 2, "", "", "بارگذاری و تبادل اسناد خطرناک", "Danger Upload", null, null, null, null, null, true, false, true, 1m, "", true, null },
                    { 111, null, null, null, 1, null, 2, "", "", "کنسلی کانتینرهای صادراتی", "Export Container Cancel", null, null, null, null, null, true, false, true, 1m, "", true, null },
                    { 112, null, null, null, 1, null, 2, "", "", "شیفتینگ کانتینر در چابهار", "Chabahar Container Shifting", null, null, null, null, null, true, false, true, 1m, "", true, null }
                });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "FinanceDirections",
                keyColumn: "Id",
                keyValue: 1,
                column: "InvoiceContextName",
                value: "Sell(فروش)");

            migrationBuilder.UpdateData(
                schema: "base",
                table: "FinanceDirections",
                keyColumn: "Id",
                keyValue: 2,
                column: "InvoiceContextName",
                value: "Buy(خرید)");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CreateUserId",
                schema: "fin",
                table: "Invoices",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_EditUserId",
                schema: "fin",
                table: "Invoices",
                column: "EditUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_CreateUserId",
                schema: "fin",
                table: "Invoices",
                column: "CreateUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Users_EditUserId",
                schema: "fin",
                table: "Invoices",
                column: "EditUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_CreateUserId",
                schema: "fin",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Users_EditUserId",
                schema: "fin",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CreateUserId",
                schema: "fin",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_EditUserId",
                schema: "fin",
                table: "Invoices");

            migrationBuilder.DeleteData(
                schema: "base",
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentAttachmentFileName",
                schema: "ops",
                table: "DocumentAttachments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.InsertData(
                schema: "base",
                table: "ChargeTypes",
                columns: new[] { "Id", "AffectProfit", "AllowManualAmount", "AllowRateBased", "CalculationById", "CalculationTypeId", "ChargeTypeCategoryId", "ChargeTypeCode", "ChargeTypeCode2", "ChargeTypeLocalName", "ChargeTypeName", "CreateUserId", "CreatedAt", "CurrencyId", "DependsOnChargeTypeId", "EditUserId", "IsActive", "IsDeleted", "IsTaxable", "MultiplierRate", "Remarks", "SystemDefined", "UpdatedAt" },
                values: new object[] { 17, null, null, null, 1, null, 1, "", "", "پلمپ", "Container Seal", null, null, null, null, null, true, false, true, 1m, "", true, null });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "FinanceDirections",
                keyColumn: "Id",
                keyValue: 1,
                column: "InvoiceContextName",
                value: "Sell(درآمد)");

            migrationBuilder.UpdateData(
                schema: "base",
                table: "FinanceDirections",
                keyColumn: "Id",
                keyValue: 2,
                column: "InvoiceContextName",
                value: "Buy(هزینه)");
        }
    }
}

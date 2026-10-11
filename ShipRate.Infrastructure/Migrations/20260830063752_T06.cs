using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "base",
                table: "ChargeTypes",
                columns: new[] { "Id", "AffectProfit", "AllowManualAmount", "AllowRateBased", "CalculationById", "CalculationTypeId", "ChargeTypeCategoryId", "ChargeTypeCode", "ChargeTypeCode2", "ChargeTypeLocalName", "ChargeTypeName", "CreateUserId", "CreatedAt", "CurrencyId", "DependsOnChargeTypeId", "EditUserId", "IsActive", "IsDeleted", "IsTaxable", "MultiplierRate", "Remarks", "SystemDefined", "UpdatedAt" },
                values: new object[] { 155, null, null, null, 1, null, 4, "", "", "تی اچ سی", "THC", null, null, null, null, null, true, false, true, 1m, "", true, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "base",
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 155);
        }
    }
}

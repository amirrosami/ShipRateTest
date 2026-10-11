using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "base",
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.AddColumn<decimal>(
                name: "AllocatedAmount",
                schema: "fin",
                table: "PaymentDocuments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SettledAmount",
                schema: "fin",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                schema: "base",
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "PaymentMethodLocalName", "PaymentMethodName" },
                values: new object[] { "واریز به حساب", "DepositSlip" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "ترانشیپ", "Tranship" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "مرجوعی", "Return" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "واردات کانتینر خالی", "Import MT Container" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "منطقه ویژه رجائی", "Rajaee S.E.Z" });

            migrationBuilder.InsertData(
                schema: "base",
                table: "TradeTypes",
                columns: new[] { "Id", "IsActive", "IsDeleted", "SortOrder", "TradeTypeLocalName", "TradeTypeName" },
                values: new object[,]
                {
                    { 20, true, false, 0, "کراس ترید", "Cross Trade" },
                    { 21, true, false, 0, "حمل داخلی", "Domestic" },
                    { 22, true, false, 0, "صادرات مجدد", "Re-Export" },
                    { 23, true, false, 0, "ورود موقت", "Temporary" },
                    { 24, true, false, 0, "کابوتاژ", "Cabotage" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DropColumn(
                name: "AllocatedAmount",
                schema: "fin",
                table: "PaymentDocuments");

            migrationBuilder.DropColumn(
                name: "SettledAmount",
                schema: "fin",
                table: "Invoices");

            migrationBuilder.UpdateData(
                schema: "base",
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "PaymentMethodLocalName", "PaymentMethodName" },
                values: new object[] { "PromissoryNote", "PromissoryNote" });

            migrationBuilder.InsertData(
                schema: "base",
                table: "PaymentMethods",
                columns: new[] { "Id", "IsActive", "IsDeleted", "PaymentMethodLocalName", "PaymentMethodName", "PaymentMethodRemarks" },
                values: new object[] { 7, true, false, "DepositSlip", "DepositSlip", "" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "کراس ترید", "Cross Trade" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "حمل داخلی", "Domestic" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "صادرات مجدد", "Re-Export" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "ورود موقت", "Temporary" });

            migrationBuilder.InsertData(
                schema: "base",
                table: "TradeTypes",
                columns: new[] { "Id", "IsActive", "IsDeleted", "SortOrder", "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { 9, true, false, 0, "کابوتاژ", "Cabotage" });
        }
    }
}

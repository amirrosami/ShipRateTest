using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "ترانزیت داخلی", "Internal Transit" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "ترانزیت خارجی", "External Transit" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "کراس ترید", "Cross Trade" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "حمل داخلی", "Domestic" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "ترانزیت", "Transit" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "صادرات مجدد", "Re-Export" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "ورود موقت", "Temporary" });

            migrationBuilder.UpdateData(
                schema: "base",
                table: "TradeTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "TradeTypeLocalName", "TradeTypeName" },
                values: new object[] { "کابوتاژ", "Cabotage" });
        }
    }
}

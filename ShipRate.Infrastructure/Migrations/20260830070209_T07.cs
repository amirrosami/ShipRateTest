using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T07 : Migration
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
                    { 450, true, false, "تعرفه ها", "Tariffs" },
                    { 460, true, false, "مکاتبات", "Correspondences" },
                    { 470, true, false, "مرسوله های پستی", "PostalDispatches" },
                    { 480, true, false, "رانندگان", "Drivers" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 450);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 460);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 470);

            migrationBuilder.DeleteData(
                schema: "base",
                table: "SystemObjectTypes",
                keyColumn: "Id",
                keyValue: 480);
        }
    }
}

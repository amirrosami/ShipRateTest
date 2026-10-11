using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T39 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "fin",
                table: "CashPaymentRequestStatus",
                columns: new[] { "Id", "CashPaymentRequestStatusLocalName", "CashPaymentRequestStatusName", "IsActive", "IsDeleted" },
                values: new object[,]
                {
                    { 5, "تایید اولیه شده", "Initial Confirmed", true, false },
                    { 6, "تایید نهایی شده", "Final Confirmed", true, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "fin",
                table: "CashPaymentRequestStatus",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "fin",
                table: "CashPaymentRequestStatus",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}

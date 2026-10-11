using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "fin",
                table: "InvoiceSubjects",
                columns: new[] { "Id", "InvoiceSubjectLocalName", "InvoiceSubjectName", "IsActive", "IsDeleted" },
                values: new object[] { 4, "صورتحساب هزینه های محلی", "DO Invoice", true, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "fin",
                table: "InvoiceSubjects",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}

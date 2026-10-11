using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T36 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinal",
                schema: "ops",
                table: "BLVoyages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SerialPrefix_Serial",
                schema: "fin",
                table: "Invoices",
                columns: new[] { "SerialPrefix", "Serial", "CompanyId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_SerialPrefix_Serial",
                schema: "fin",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsFinal",
                schema: "ops",
                table: "BLVoyages");
        }
    }
}

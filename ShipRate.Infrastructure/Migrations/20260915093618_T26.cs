using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SysCalcInitialValue",
                schema: "ops",
                table: "NOAPrices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SysCalcTotalValue",
                schema: "ops",
                table: "NOAPrices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SysCalcVATAmount",
                schema: "ops",
                table: "NOAPrices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InOutId",
                schema: "ops",
                table: "NOA",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SysCalcInitialValue",
                schema: "ops",
                table: "NOAPrices");

            migrationBuilder.DropColumn(
                name: "SysCalcTotalValue",
                schema: "ops",
                table: "NOAPrices");

            migrationBuilder.DropColumn(
                name: "SysCalcVATAmount",
                schema: "ops",
                table: "NOAPrices");

            migrationBuilder.DropColumn(
                name: "InOutId",
                schema: "ops",
                table: "NOA");
        }
    }
}

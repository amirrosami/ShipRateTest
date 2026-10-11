using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FullMTId",
                schema: "tariff",
                table: "TariffRules",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TariffRules_FullMTId",
                schema: "tariff",
                table: "TariffRules",
                column: "FullMTId");

            migrationBuilder.AddForeignKey(
                name: "FK_TariffRules_FullMT_FullMTId",
                schema: "tariff",
                table: "TariffRules",
                column: "FullMTId",
                principalSchema: "base",
                principalTable: "FullMT",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TariffRules_FullMT_FullMTId",
                schema: "tariff",
                table: "TariffRules");

            migrationBuilder.DropIndex(
                name: "IX_TariffRules_FullMTId",
                schema: "tariff",
                table: "TariffRules");

            migrationBuilder.DropColumn(
                name: "FullMTId",
                schema: "tariff",
                table: "TariffRules");
        }
    }
}

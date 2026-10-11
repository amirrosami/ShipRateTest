using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDG",
                schema: "tariff",
                table: "TariffRules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverSized",
                schema: "tariff",
                table: "TariffRules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverWeight",
                schema: "tariff",
                table: "TariffRules",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TradeTypeId",
                schema: "tariff",
                table: "TariffRules",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TariffRules_TradeTypeId",
                schema: "tariff",
                table: "TariffRules",
                column: "TradeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TariffRules_TradeTypes_TradeTypeId",
                schema: "tariff",
                table: "TariffRules",
                column: "TradeTypeId",
                principalSchema: "base",
                principalTable: "TradeTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TariffRules_TradeTypes_TradeTypeId",
                schema: "tariff",
                table: "TariffRules");

            migrationBuilder.DropIndex(
                name: "IX_TariffRules_TradeTypeId",
                schema: "tariff",
                table: "TariffRules");

            migrationBuilder.DropColumn(
                name: "IsDG",
                schema: "tariff",
                table: "TariffRules");

            migrationBuilder.DropColumn(
                name: "IsOverSized",
                schema: "tariff",
                table: "TariffRules");

            migrationBuilder.DropColumn(
                name: "IsOverWeight",
                schema: "tariff",
                table: "TariffRules");

            migrationBuilder.DropColumn(
                name: "TradeTypeId",
                schema: "tariff",
                table: "TariffRules");
        }
    }
}

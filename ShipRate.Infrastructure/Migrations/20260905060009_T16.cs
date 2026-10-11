using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TradeTypeId",
                schema: "ops",
                table: "BLs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BLs_TradeTypeId",
                schema: "ops",
                table: "BLs",
                column: "TradeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BLs_TradeTypes_TradeTypeId",
                schema: "ops",
                table: "BLs",
                column: "TradeTypeId",
                principalSchema: "base",
                principalTable: "TradeTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BLs_TradeTypes_TradeTypeId",
                schema: "ops",
                table: "BLs");

            migrationBuilder.DropIndex(
                name: "IX_BLs_TradeTypeId",
                schema: "ops",
                table: "BLs");

            migrationBuilder.DropColumn(
                name: "TradeTypeId",
                schema: "ops",
                table: "BLs");
        }
    }
}

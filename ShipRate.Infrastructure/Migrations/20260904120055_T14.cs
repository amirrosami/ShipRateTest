using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BLId",
                schema: "fin",
                table: "Settlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DetentionId",
                schema: "fin",
                table: "SettlementItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_BLId",
                schema: "fin",
                table: "Settlements",
                column: "BLId");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementItems_DetentionId",
                schema: "fin",
                table: "SettlementItems",
                column: "DetentionId");

            migrationBuilder.CreateIndex(
                name: "IX_NOA_CreateUserId",
                schema: "ops",
                table: "NOA",
                column: "CreateUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_NOA_Users_CreateUserId",
                schema: "ops",
                table: "NOA",
                column: "CreateUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SettlementItems_Detentions_DetentionId",
                schema: "fin",
                table: "SettlementItems",
                column: "DetentionId",
                principalSchema: "cnt",
                principalTable: "Detentions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Settlements_BLs_BLId",
                schema: "fin",
                table: "Settlements",
                column: "BLId",
                principalSchema: "ops",
                principalTable: "BLs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NOA_Users_CreateUserId",
                schema: "ops",
                table: "NOA");

            migrationBuilder.DropForeignKey(
                name: "FK_SettlementItems_Detentions_DetentionId",
                schema: "fin",
                table: "SettlementItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Settlements_BLs_BLId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.DropIndex(
                name: "IX_Settlements_BLId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.DropIndex(
                name: "IX_SettlementItems_DetentionId",
                schema: "fin",
                table: "SettlementItems");

            migrationBuilder.DropIndex(
                name: "IX_NOA_CreateUserId",
                schema: "ops",
                table: "NOA");

            migrationBuilder.DropColumn(
                name: "BLId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "DetentionId",
                schema: "fin",
                table: "SettlementItems");
        }
    }
}

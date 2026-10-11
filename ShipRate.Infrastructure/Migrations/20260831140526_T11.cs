using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConditionText",
                schema: "ops",
                table: "NOAPrices");

            migrationBuilder.DropColumn(
                name: "IsConditional",
                schema: "ops",
                table: "NOAPrices");

            migrationBuilder.AddColumn<int>(
                name: "ClearanceTypeId",
                schema: "ops",
                table: "NOAPrices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NOAPrices_ClearanceTypeId",
                schema: "ops",
                table: "NOAPrices",
                column: "ClearanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetentionItems_ContainerSizeTypeId",
                schema: "cnt",
                table: "DetentionItems",
                column: "ContainerSizeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetentionItems_ContainerSizeTypes_ContainerSizeTypeId",
                schema: "cnt",
                table: "DetentionItems",
                column: "ContainerSizeTypeId",
                principalSchema: "base",
                principalTable: "ContainerSizeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NOAPrices_ClearanceType_ClearanceTypeId",
                schema: "ops",
                table: "NOAPrices",
                column: "ClearanceTypeId",
                principalSchema: "base",
                principalTable: "ClearanceType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetentionItems_ContainerSizeTypes_ContainerSizeTypeId",
                schema: "cnt",
                table: "DetentionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_NOAPrices_ClearanceType_ClearanceTypeId",
                schema: "ops",
                table: "NOAPrices");

            migrationBuilder.DropIndex(
                name: "IX_NOAPrices_ClearanceTypeId",
                schema: "ops",
                table: "NOAPrices");

            migrationBuilder.DropIndex(
                name: "IX_DetentionItems_ContainerSizeTypeId",
                schema: "cnt",
                table: "DetentionItems");

            migrationBuilder.DropColumn(
                name: "ClearanceTypeId",
                schema: "ops",
                table: "NOAPrices");

            migrationBuilder.AddColumn<string>(
                name: "ConditionText",
                schema: "ops",
                table: "NOAPrices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConditional",
                schema: "ops",
                table: "NOAPrices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

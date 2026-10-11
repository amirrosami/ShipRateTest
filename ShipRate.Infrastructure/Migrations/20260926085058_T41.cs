using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrders_DOOperationalModes_DOOperationalModeId",
                schema: "ops",
                table: "DeliveryOrders");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrders_CompanyId",
                schema: "ops",
                table: "DeliveryOrders");

            migrationBuilder.AlterColumn<int>(
                name: "DOOperationalModeId",
                schema: "ops",
                table: "DeliveryOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Serial",
                schema: "ops",
                table: "DeliveryOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrders_CompanyId_Serial_DOOperationalModeId",
                schema: "ops",
                table: "DeliveryOrders",
                columns: new[] { "CompanyId", "Serial", "DOOperationalModeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrders_DOOperationalModes_DOOperationalModeId",
                schema: "ops",
                table: "DeliveryOrders",
                column: "DOOperationalModeId",
                principalSchema: "base",
                principalTable: "DOOperationalModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrders_DOOperationalModes_DOOperationalModeId",
                schema: "ops",
                table: "DeliveryOrders");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrders_CompanyId_Serial_DOOperationalModeId",
                schema: "ops",
                table: "DeliveryOrders");

            migrationBuilder.DropColumn(
                name: "Serial",
                schema: "ops",
                table: "DeliveryOrders");

            migrationBuilder.AlterColumn<int>(
                name: "DOOperationalModeId",
                schema: "ops",
                table: "DeliveryOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrders_CompanyId",
                schema: "ops",
                table: "DeliveryOrders",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrders_DOOperationalModes_DOOperationalModeId",
                schema: "ops",
                table: "DeliveryOrders",
                column: "DOOperationalModeId",
                principalSchema: "base",
                principalTable: "DOOperationalModes",
                principalColumn: "Id");
        }
    }
}

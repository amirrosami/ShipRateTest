using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RateRequests_Customers_CustomerId",
                schema: "ops",
                table: "RateRequests");

            migrationBuilder.AddColumn<bool>(
                name: "ExternalTransitRequired",
                schema: "ops",
                table: "RateRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InternalTransitRequired",
                schema: "ops",
                table: "RateRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RateRequests_Persons_CustomerId",
                schema: "ops",
                table: "RateRequests",
                column: "CustomerId",
                principalSchema: "base",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RateRequests_Persons_CustomerId",
                schema: "ops",
                table: "RateRequests");

            migrationBuilder.DropColumn(
                name: "ExternalTransitRequired",
                schema: "ops",
                table: "RateRequests");

            migrationBuilder.DropColumn(
                name: "InternalTransitRequired",
                schema: "ops",
                table: "RateRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_RateRequests_Customers_CustomerId",
                schema: "ops",
                table: "RateRequests",
                column: "CustomerId",
                principalSchema: "crm",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}

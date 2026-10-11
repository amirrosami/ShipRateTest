using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T46 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                schema: "fin",
                table: "Settlements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_BookingId",
                schema: "fin",
                table: "Settlements",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Settlements_Bookings_BookingId",
                schema: "fin",
                table: "Settlements",
                column: "BookingId",
                principalSchema: "ops",
                principalTable: "Bookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Settlements_Bookings_BookingId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.DropIndex(
                name: "IX_Settlements_BookingId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "BookingId",
                schema: "fin",
                table: "Settlements");
        }
    }
}

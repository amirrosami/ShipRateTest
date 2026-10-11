using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContainerUses_ContainerId",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                schema: "cnt",
                table: "ContainerUses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CycleCloseReason",
                schema: "cnt",
                table: "ContainerUses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CycleStartDate",
                schema: "cnt",
                table: "ContainerUses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EIRNumber",
                schema: "cnt",
                table: "ContainerUses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RTSInstructionId",
                schema: "cnt",
                table: "ContainerUses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondSealNumber",
                schema: "cnt",
                table: "ContainerUses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContainerUses_BookingId",
                schema: "cnt",
                table: "ContainerUses",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerUses_ContainerId_CycleClosed",
                schema: "cnt",
                table: "ContainerUses",
                columns: new[] { "ContainerId", "CycleClosed" });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerUses_ContainerNumber_CycleClosed",
                schema: "cnt",
                table: "ContainerUses",
                columns: new[] { "ContainerNumber", "CycleClosed" });

            migrationBuilder.CreateIndex(
                name: "IX_ContainerUses_RTSInstructionId",
                schema: "cnt",
                table: "ContainerUses",
                column: "RTSInstructionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerUses_Bookings_BookingId",
                schema: "cnt",
                table: "ContainerUses",
                column: "BookingId",
                principalSchema: "ops",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerUses_RTSInstructions_RTSInstructionId",
                schema: "cnt",
                table: "ContainerUses",
                column: "RTSInstructionId",
                principalSchema: "ops",
                principalTable: "RTSInstructions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContainerUses_Bookings_BookingId",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropForeignKey(
                name: "FK_ContainerUses_RTSInstructions_RTSInstructionId",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropIndex(
                name: "IX_ContainerUses_BookingId",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropIndex(
                name: "IX_ContainerUses_ContainerId_CycleClosed",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropIndex(
                name: "IX_ContainerUses_ContainerNumber_CycleClosed",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropIndex(
                name: "IX_ContainerUses_RTSInstructionId",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropColumn(
                name: "BookingId",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropColumn(
                name: "CycleCloseReason",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropColumn(
                name: "CycleStartDate",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropColumn(
                name: "EIRNumber",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropColumn(
                name: "RTSInstructionId",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.DropColumn(
                name: "SecondSealNumber",
                schema: "cnt",
                table: "ContainerUses");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerUses_ContainerId",
                schema: "cnt",
                table: "ContainerUses",
                column: "ContainerId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                schema: "cnt",
                table: "ContainerActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RTSInstructionId",
                schema: "cnt",
                table: "ContainerActions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RTSInstructions_CreateUserId",
                schema: "ops",
                table: "RTSInstructions",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RTSInstructions_EditUserId",
                schema: "ops",
                table: "RTSInstructions",
                column: "EditUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerActions_BookingId",
                schema: "cnt",
                table: "ContainerActions",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerActions_RTSInstructionId",
                schema: "cnt",
                table: "ContainerActions",
                column: "RTSInstructionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerActions_Bookings_BookingId",
                schema: "cnt",
                table: "ContainerActions",
                column: "BookingId",
                principalSchema: "ops",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContainerActions_RTSInstructions_RTSInstructionId",
                schema: "cnt",
                table: "ContainerActions",
                column: "RTSInstructionId",
                principalSchema: "ops",
                principalTable: "RTSInstructions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RTSInstructions_Users_CreateUserId",
                schema: "ops",
                table: "RTSInstructions",
                column: "CreateUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RTSInstructions_Users_EditUserId",
                schema: "ops",
                table: "RTSInstructions",
                column: "EditUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContainerActions_Bookings_BookingId",
                schema: "cnt",
                table: "ContainerActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ContainerActions_RTSInstructions_RTSInstructionId",
                schema: "cnt",
                table: "ContainerActions");

            migrationBuilder.DropForeignKey(
                name: "FK_RTSInstructions_Users_CreateUserId",
                schema: "ops",
                table: "RTSInstructions");

            migrationBuilder.DropForeignKey(
                name: "FK_RTSInstructions_Users_EditUserId",
                schema: "ops",
                table: "RTSInstructions");

            migrationBuilder.DropIndex(
                name: "IX_RTSInstructions_CreateUserId",
                schema: "ops",
                table: "RTSInstructions");

            migrationBuilder.DropIndex(
                name: "IX_RTSInstructions_EditUserId",
                schema: "ops",
                table: "RTSInstructions");

            migrationBuilder.DropIndex(
                name: "IX_ContainerActions_BookingId",
                schema: "cnt",
                table: "ContainerActions");

            migrationBuilder.DropIndex(
                name: "IX_ContainerActions_RTSInstructionId",
                schema: "cnt",
                table: "ContainerActions");

            migrationBuilder.DropColumn(
                name: "BookingId",
                schema: "cnt",
                table: "ContainerActions");

            migrationBuilder.DropColumn(
                name: "RTSInstructionId",
                schema: "cnt",
                table: "ContainerActions");
        }
    }
}

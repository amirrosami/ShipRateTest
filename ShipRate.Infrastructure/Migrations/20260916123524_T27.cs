using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SmsMessageId",
                schema: "ops",
                table: "NOASend",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NOASend_SmsMessageId",
                schema: "ops",
                table: "NOASend",
                column: "SmsMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_NOASend_SmsMessages_SmsMessageId",
                schema: "ops",
                table: "NOASend",
                column: "SmsMessageId",
                principalSchema: "comms",
                principalTable: "SmsMessages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NOASend_SmsMessages_SmsMessageId",
                schema: "ops",
                table: "NOASend");

            migrationBuilder.DropIndex(
                name: "IX_NOASend_SmsMessageId",
                schema: "ops",
                table: "NOASend");

            migrationBuilder.DropColumn(
                name: "SmsMessageId",
                schema: "ops",
                table: "NOASend");
        }
    }
}

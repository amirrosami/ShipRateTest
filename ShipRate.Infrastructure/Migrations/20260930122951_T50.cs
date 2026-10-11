using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectComments",
                schema: "ops",
                table: "RateRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectDate",
                schema: "ops",
                table: "RateRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectUserId",
                schema: "ops",
                table: "RateRequests",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectComments",
                schema: "ops",
                table: "RateRequests");

            migrationBuilder.DropColumn(
                name: "RejectDate",
                schema: "ops",
                table: "RateRequests");

            migrationBuilder.DropColumn(
                name: "RejectUserId",
                schema: "ops",
                table: "RateRequests");
        }
    }
}

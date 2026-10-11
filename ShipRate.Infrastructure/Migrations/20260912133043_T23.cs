using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BLContainerTripAllocations",
                schema: "ops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BLContainerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BLTripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BLContainerTripAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BLContainerTripAllocations_BLContainers_BLContainerId",
                        column: x => x.BLContainerId,
                        principalSchema: "ops",
                        principalTable: "BLContainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BLContainerTripAllocations_BLTrips_BLTripId",
                        column: x => x.BLTripId,
                        principalSchema: "ops",
                        principalTable: "BLTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BLContainerTripAllocations_BLContainerId",
                schema: "ops",
                table: "BLContainerTripAllocations",
                column: "BLContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_BLContainerTripAllocations_BLTripId",
                schema: "ops",
                table: "BLContainerTripAllocations",
                column: "BLTripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BLContainerTripAllocations",
                schema: "ops");
        }
    }
}

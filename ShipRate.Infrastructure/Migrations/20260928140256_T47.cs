using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T47 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FreightLetters",
                schema: "ops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DOId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BLId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MBLNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HBLNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomsOfficeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FreightValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    FromLocationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToLocationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LetterNo = table.Column<int>(type: "int", nullable: false),
                    LetterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_FreightLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FreightLetters_BLs_BLId",
                        column: x => x.BLId,
                        principalSchema: "ops",
                        principalTable: "BLs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FreightLetters_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "base",
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FreightLetters_DeliveryOrders_DOId",
                        column: x => x.DOId,
                        principalSchema: "ops",
                        principalTable: "DeliveryOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FreightLetters_Locations_CustomsOfficeId",
                        column: x => x.CustomsOfficeId,
                        principalSchema: "base",
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FreightLetters_BLId",
                schema: "ops",
                table: "FreightLetters",
                column: "BLId");

            migrationBuilder.CreateIndex(
                name: "IX_FreightLetters_CurrencyId",
                schema: "ops",
                table: "FreightLetters",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_FreightLetters_CustomsOfficeId",
                schema: "ops",
                table: "FreightLetters",
                column: "CustomsOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_FreightLetters_DOId",
                schema: "ops",
                table: "FreightLetters",
                column: "DOId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FreightLetters",
                schema: "ops");
        }
    }
}

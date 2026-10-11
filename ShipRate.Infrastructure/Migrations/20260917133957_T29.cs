using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T29 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PreviousPaidAmount",
                schema: "cnt",
                table: "Detentions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Detentions_CreateUserId",
                schema: "cnt",
                table: "Detentions",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Detentions_EditUserId",
                schema: "cnt",
                table: "Detentions",
                column: "EditUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_Users_CreateUserId",
                schema: "cnt",
                table: "Detentions",
                column: "CreateUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_Users_EditUserId",
                schema: "cnt",
                table: "Detentions",
                column: "EditUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_Users_CreateUserId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_Users_EditUserId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropIndex(
                name: "IX_Detentions_CreateUserId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropIndex(
                name: "IX_Detentions_EditUserId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropColumn(
                name: "PreviousPaidAmount",
                schema: "cnt",
                table: "Detentions");
        }
    }
}

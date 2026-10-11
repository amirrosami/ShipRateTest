using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SettlementNo",
                schema: "fin",
                table: "Settlements",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR dbo.Seq_SettlementNo",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_CreateUserId",
                schema: "fin",
                table: "Settlements",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_EditUserId",
                schema: "fin",
                table: "Settlements",
                column: "EditUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Settlements_Users_CreateUserId",
                schema: "fin",
                table: "Settlements",
                column: "CreateUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Settlements_Users_EditUserId",
                schema: "fin",
                table: "Settlements",
                column: "EditUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Settlements_Users_CreateUserId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.DropForeignKey(
                name: "FK_Settlements_Users_EditUserId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.DropIndex(
                name: "IX_Settlements_CreateUserId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.DropIndex(
                name: "IX_Settlements_EditUserId",
                schema: "fin",
                table: "Settlements");

            migrationBuilder.AlterColumn<int>(
                name: "SettlementNo",
                schema: "fin",
                table: "Settlements",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR dbo.Seq_SettlementNo");
        }
    }
}

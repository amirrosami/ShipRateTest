using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DetentionNo",
                schema: "cnt",
                table: "Detentions",
                type: "bigint",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR dbo.Seq_DetentionCalculationNo",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Detentions_DetentionNo",
                schema: "cnt",
                table: "Detentions",
                column: "DetentionNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Detentions_DetentionNo",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.AlterColumn<long>(
                name: "DetentionNo",
                schema: "cnt",
                table: "Detentions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "NEXT VALUE FOR dbo.Seq_DetentionCalculationNo");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SystemId",
                schema: "fin",
                table: "PaymentDocuments",
                type: "bigint",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR dbo.Seq_PaymentSystemId",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDocuments_SystemId",
                schema: "fin",
                table: "PaymentDocuments",
                column: "SystemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentDocuments_SystemId",
                schema: "fin",
                table: "PaymentDocuments");

            migrationBuilder.AlterColumn<long>(
                name: "SystemId",
                schema: "fin",
                table: "PaymentDocuments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "NEXT VALUE FOR dbo.Seq_PaymentSystemId")
                .Annotation("SqlServer:Identity", "100000, 1");
        }
    }
}

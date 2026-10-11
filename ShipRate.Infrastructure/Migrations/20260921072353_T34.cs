using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "fin",
                table: "InvoiceSubjects",
                columns: new[] { "Id", "InvoiceSubjectLocalName", "InvoiceSubjectName", "IsActive", "IsDeleted" },
                values: new object[] { 10, "صورتحساب سپرده ها", "Warranty Invoice", true, false });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDocuments_CreateUserId",
                schema: "fin",
                table: "PaymentDocuments",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDocuments_EditUserId",
                schema: "fin",
                table: "PaymentDocuments",
                column: "EditUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentDocuments_Users_CreateUserId",
                schema: "fin",
                table: "PaymentDocuments",
                column: "CreateUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentDocuments_Users_EditUserId",
                schema: "fin",
                table: "PaymentDocuments",
                column: "EditUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentDocuments_Users_CreateUserId",
                schema: "fin",
                table: "PaymentDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentDocuments_Users_EditUserId",
                schema: "fin",
                table: "PaymentDocuments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentDocuments_CreateUserId",
                schema: "fin",
                table: "PaymentDocuments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentDocuments_EditUserId",
                schema: "fin",
                table: "PaymentDocuments");

            migrationBuilder.DeleteData(
                schema: "fin",
                table: "InvoiceSubjects",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_Currency_PaymentCurrencyId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_Persons_IssuedToPartyId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_Persons_IssuerPartyId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropColumn(
                name: "IssuedByUs",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.RenameColumn(
                name: "PaymentCurrencyId",
                schema: "cnt",
                table: "Detentions",
                newName: "FinanceDirectionId");

            migrationBuilder.RenameColumn(
                name: "IssuerPartyId",
                schema: "cnt",
                table: "Detentions",
                newName: "PrevDetentionId");

            migrationBuilder.RenameColumn(
                name: "IssuedToPartyId",
                schema: "cnt",
                table: "Detentions",
                newName: "PartyId");

            migrationBuilder.RenameIndex(
                name: "IX_Detentions_PaymentCurrencyId",
                schema: "cnt",
                table: "Detentions",
                newName: "IX_Detentions_FinanceDirectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Detentions_IssuerPartyId",
                schema: "cnt",
                table: "Detentions",
                newName: "IX_Detentions_PrevDetentionId");

            migrationBuilder.RenameIndex(
                name: "IX_Detentions_IssuedToPartyId",
                schema: "cnt",
                table: "Detentions",
                newName: "IX_Detentions_PartyId");

            migrationBuilder.AddColumn<int>(
                name: "FinalCurrencyId",
                schema: "cnt",
                table: "Detentions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPayableAmount",
                schema: "cnt",
                table: "Detentions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalVATAmount",
                schema: "cnt",
                table: "Detentions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SettledAmount",
                schema: "cnt",
                table: "Detentions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATPercent",
                schema: "cnt",
                table: "Detentions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Detentions_FinalCurrencyId",
                schema: "cnt",
                table: "Detentions",
                column: "FinalCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_Currency_FinalCurrencyId",
                schema: "cnt",
                table: "Detentions",
                column: "FinalCurrencyId",
                principalSchema: "base",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_Detentions_PrevDetentionId",
                schema: "cnt",
                table: "Detentions",
                column: "PrevDetentionId",
                principalSchema: "cnt",
                principalTable: "Detentions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_FinanceDirections_FinanceDirectionId",
                schema: "cnt",
                table: "Detentions",
                column: "FinanceDirectionId",
                principalSchema: "base",
                principalTable: "FinanceDirections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_Persons_PartyId",
                schema: "cnt",
                table: "Detentions",
                column: "PartyId",
                principalSchema: "base",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_Currency_FinalCurrencyId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_Detentions_PrevDetentionId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_FinanceDirections_FinanceDirectionId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropForeignKey(
                name: "FK_Detentions_Persons_PartyId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropIndex(
                name: "IX_Detentions_FinalCurrencyId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropColumn(
                name: "FinalCurrencyId",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropColumn(
                name: "FinalPayableAmount",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropColumn(
                name: "FinalVATAmount",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropColumn(
                name: "SettledAmount",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.DropColumn(
                name: "VATPercent",
                schema: "cnt",
                table: "Detentions");

            migrationBuilder.RenameColumn(
                name: "PrevDetentionId",
                schema: "cnt",
                table: "Detentions",
                newName: "IssuerPartyId");

            migrationBuilder.RenameColumn(
                name: "PartyId",
                schema: "cnt",
                table: "Detentions",
                newName: "IssuedToPartyId");

            migrationBuilder.RenameColumn(
                name: "FinanceDirectionId",
                schema: "cnt",
                table: "Detentions",
                newName: "PaymentCurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Detentions_PrevDetentionId",
                schema: "cnt",
                table: "Detentions",
                newName: "IX_Detentions_IssuerPartyId");

            migrationBuilder.RenameIndex(
                name: "IX_Detentions_PartyId",
                schema: "cnt",
                table: "Detentions",
                newName: "IX_Detentions_IssuedToPartyId");

            migrationBuilder.RenameIndex(
                name: "IX_Detentions_FinanceDirectionId",
                schema: "cnt",
                table: "Detentions",
                newName: "IX_Detentions_PaymentCurrencyId");

            migrationBuilder.AddColumn<bool>(
                name: "IssuedByUs",
                schema: "cnt",
                table: "Detentions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_Currency_PaymentCurrencyId",
                schema: "cnt",
                table: "Detentions",
                column: "PaymentCurrencyId",
                principalSchema: "base",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_Persons_IssuedToPartyId",
                schema: "cnt",
                table: "Detentions",
                column: "IssuedToPartyId",
                principalSchema: "base",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Detentions_Persons_IssuerPartyId",
                schema: "cnt",
                table: "Detentions",
                column: "IssuerPartyId",
                principalSchema: "base",
                principalTable: "Persons",
                principalColumn: "Id");
        }
    }
}

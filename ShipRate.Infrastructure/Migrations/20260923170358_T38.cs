using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaultAssets_AssetTypes_AssetTypeId",
                schema: "fin",
                table: "VaultAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_VaultMovements_AssetTypes_AssetTypeId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_VaultMovements_Users_AuthorizedByUserId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_VaultMovements_VaultAssets_VaultAssetId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_VaultMovements_VaultMovementStatus_StatusId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropTable(
                name: "AssetTypes",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "VaultAcknowledges",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "VaultMovementStatus",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "AssetCategory",
                schema: "fin");

            migrationBuilder.DropIndex(
                name: "IX_VaultMovements_AuthorizedByUserId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropIndex(
                name: "IX_VaultMovements_VaultAssetId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropIndex(
                name: "IX_VaultMovements_VaultId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropIndex(
                name: "IX_VaultAssets_VaultId",
                schema: "fin",
                table: "VaultAssets");

            migrationBuilder.DropColumn(
                name: "AuthorizeDate",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropColumn(
                name: "AuthorizedByUserId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropColumn(
                name: "FirstConfirmDate",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropColumn(
                name: "FirstConfirmedByUserId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                schema: "fin",
                table: "VaultAssets");

            migrationBuilder.RenameColumn(
                name: "VaultAssetId",
                schema: "fin",
                table: "VaultMovements",
                newName: "SourceEntityId");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                schema: "fin",
                table: "VaultMovements",
                newName: "VaultMovementSourceTypeId");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                schema: "fin",
                table: "VaultMovements",
                newName: "CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_VaultMovements_StatusId",
                schema: "fin",
                table: "VaultMovements",
                newName: "IX_VaultMovements_VaultMovementSourceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_VaultMovements_AssetTypeId",
                schema: "fin",
                table: "VaultMovements",
                newName: "IX_VaultMovements_CurrencyId");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                schema: "fin",
                table: "VaultAssets",
                newName: "CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_VaultAssets_AssetTypeId",
                schema: "fin",
                table: "VaultAssets",
                newName: "IX_VaultAssets_CurrencyId");

            migrationBuilder.AlterColumn<string>(
                name: "VaultName",
                schema: "fin",
                table: "Vaults",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "fin",
                table: "Vaults",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "fin",
                table: "Vaults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "KeeperUserId",
                schema: "fin",
                table: "Vaults",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                schema: "fin",
                table: "VaultMovements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositorOrReceiver",
                schema: "fin",
                table: "VaultMovements",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                schema: "fin",
                table: "VaultMovements",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                schema: "fin",
                table: "VaultAssets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CashPaymentRequestStatus",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CashPaymentRequestStatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CashPaymentRequestStatusLocalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashPaymentRequestStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultCustodyActionTypes",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaultCustodyActionTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaultCustodyActionTypeLocalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultCustodyActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultItemCategory",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaultItemCategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaultItemCategoryLocalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultItemCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultItemStatus",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaultItemStatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaultItemStatusLocalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultItemStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultMovementSourceTypes",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaultMovementSourceTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaultMovementSourceTypeLocalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultMovementSourceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashPaymentRequests",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PayeeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    InitialConfirmedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InitialConfirmDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalConfirmedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FinalConfirmDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaymentDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VaultMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_CashPaymentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_CashPaymentRequestStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "fin",
                        principalTable: "CashPaymentRequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "base",
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_PaymentDocuments_PaymentDocumentId",
                        column: x => x.PaymentDocumentId,
                        principalSchema: "fin",
                        principalTable: "PaymentDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_Persons_PartyId",
                        column: x => x.PartyId,
                        principalSchema: "base",
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_Users_CreateUserId",
                        column: x => x.CreateUserId,
                        principalSchema: "sec",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_Users_FinalConfirmedByUserId",
                        column: x => x.FinalConfirmedByUserId,
                        principalSchema: "sec",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_Users_InitialConfirmedByUserId",
                        column: x => x.InitialConfirmedByUserId,
                        principalSchema: "sec",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_Users_PaidByUserId",
                        column: x => x.PaidByUserId,
                        principalSchema: "sec",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_VaultMovements_VaultMovementId",
                        column: x => x.VaultMovementId,
                        principalSchema: "fin",
                        principalTable: "VaultMovements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CashPaymentRequests_Vaults_VaultId",
                        column: x => x.VaultId,
                        principalSchema: "fin",
                        principalTable: "Vaults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaultItems",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultItemCategoryId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdentifierNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OwnerOrBeneficiary = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NominalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShelfLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VaultItemStatusId = table.Column<int>(type: "int", nullable: false),
                    CurrentHolderUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentHolderExternalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CheckedOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_VaultItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultItems_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "base",
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaultItems_Users_CurrentHolderUserId",
                        column: x => x.CurrentHolderUserId,
                        principalSchema: "sec",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VaultItems_VaultItemCategory_VaultItemCategoryId",
                        column: x => x.VaultItemCategoryId,
                        principalSchema: "fin",
                        principalTable: "VaultItemCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaultItems_VaultItemStatus_VaultItemStatusId",
                        column: x => x.VaultItemStatusId,
                        principalSchema: "fin",
                        principalTable: "VaultItemStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaultItems_Vaults_VaultId",
                        column: x => x.VaultId,
                        principalSchema: "fin",
                        principalTable: "Vaults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaultItemCustodyLogs",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultCustodyActionTypeId = table.Column<int>(type: "int", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HandedToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HandedToExternalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpectedReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HandoverReceiptScanPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_VaultItemCustodyLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultItemCustodyLogs_Users_ExecutedByUserId",
                        column: x => x.ExecutedByUserId,
                        principalSchema: "sec",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaultItemCustodyLogs_Users_HandedToUserId",
                        column: x => x.HandedToUserId,
                        principalSchema: "sec",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VaultItemCustodyLogs_VaultCustodyActionTypes_VaultCustodyActionTypeId",
                        column: x => x.VaultCustodyActionTypeId,
                        principalSchema: "fin",
                        principalTable: "VaultCustodyActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaultItemCustodyLogs_VaultItems_VaultItemId",
                        column: x => x.VaultItemId,
                        principalSchema: "fin",
                        principalTable: "VaultItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "fin",
                table: "CashPaymentRequestStatus",
                columns: new[] { "Id", "CashPaymentRequestStatusLocalName", "CashPaymentRequestStatusName", "IsActive", "IsDeleted" },
                values: new object[,]
                {
                    { 1, "در انتظار پرداخت", "Pending", true, false },
                    { 2, "پرداخت شده", "Paid", true, false },
                    { 3, "رد شده", "Rejected", true, false },
                    { 4, "لغو شده", "Cancelled", true, false }
                });

            migrationBuilder.InsertData(
                schema: "fin",
                table: "VaultCustodyActionTypes",
                columns: new[] { "Id", "IsActive", "IsDeleted", "VaultCustodyActionTypeLocalName", "VaultCustodyActionTypeName" },
                values: new object[,]
                {
                    { 1, true, false, "ورود اولیه به صندوق", "InitialDeposit" },
                    { 2, true, false, "تحویل امانی", "BorrowCheckOut" },
                    { 3, true, false, "عودت به صندوق", "ReturnCheckIn" },
                    { 4, true, false, "خروج دائمی", "PermanentOut" }
                });

            migrationBuilder.InsertData(
                schema: "fin",
                table: "VaultItemCategory",
                columns: new[] { "Id", "IsActive", "IsDeleted", "VaultItemCategoryLocalName", "VaultItemCategoryName" },
                values: new object[,]
                {
                    { 1, true, false, "سند مالکیت ملکی", "TitleDeed" },
                    { 2, true, false, "اسناد شناور", "VesselDocument" },
                    { 3, true, false, "بارنامه کاغذی اصل", "OriginalBL" },
                    { 4, true, false, "ضمانت‌نامه بانکی", "BankGuarantee" },
                    { 5, true, false, "سفته ضمانت", "PromissoryNote" },
                    { 6, true, false, "چک ضمانتی", "GuaranteeCheque" },
                    { 7, true, false, "اسناد شرکتی", "CorporateCertificate" },
                    { 8, true, false, "توکن و تجهیزات", "HardwareDevice" },
                    { 99, true, false, "سایر", "Other" }
                });

            migrationBuilder.InsertData(
                schema: "fin",
                table: "VaultItemStatus",
                columns: new[] { "Id", "IsActive", "IsDeleted", "VaultItemStatusLocalName", "VaultItemStatusName" },
                values: new object[,]
                {
                    { 1, true, false, "موجود در گاوصندوق", "InVault" },
                    { 2, true, false, "به امانت رفته", "CheckedOut" },
                    { 3, true, false, "خروج قطعی", "PermanentOut" },
                    { 4, true, false, "بایگانی راکد", "Archived" }
                });

            migrationBuilder.InsertData(
                schema: "fin",
                table: "VaultMovementSourceTypes",
                columns: new[] { "Id", "IsActive", "IsDeleted", "VaultMovementSourceTypeLocalName", "VaultMovementSourceTypeName" },
                values: new object[,]
                {
                    { 1, true, false, "دریافت نقدی از مشتری", "CustomerReceipt" },
                    { 2, true, false, "شارژ از بانک", "BankToVault" },
                    { 3, true, false, "عودت تنخواه", "PettyCashSettlement" },
                    { 4, true, false, "انتقال از گاوصندوق دیگر", "InterVaultTransferIn" },
                    { 5, true, false, "استرداد سپرده", "DepositRefundIn" },
                    { 6, true, false, "مازاد شمارش فیزیکی", "PhysicalSurplus" },
                    { 7, true, false, "تزریق سرمایه", "CapitalInjection" },
                    { 10, true, false, "درخواست پرداخت نقدی", "CashPaymentRequest" },
                    { 11, true, false, "واریز به بانک", "VaultToBank" },
                    { 12, true, false, "پرداخت علی‌الحساب تنخواه", "PettyCashAdvance" },
                    { 13, true, false, "انتقال به گاوصندوق دیگر", "InterVaultTransferOut" },
                    { 14, true, false, "کسری شمارش فیزیکی", "PhysicalShortage" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vaults_BranchId",
                schema: "fin",
                table: "Vaults",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaults_KeeperUserId",
                schema: "fin",
                table: "Vaults",
                column: "KeeperUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultMovements_InOutId",
                schema: "fin",
                table: "VaultMovements",
                column: "InOutId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultMovements_MovementDate",
                schema: "fin",
                table: "VaultMovements",
                column: "MovementDate");

            migrationBuilder.CreateIndex(
                name: "IX_VaultMovements_VaultId_CurrencyId",
                schema: "fin",
                table: "VaultMovements",
                columns: new[] { "VaultId", "CurrencyId" });

            migrationBuilder.CreateIndex(
                name: "IX_VaultAssets_VaultId_CurrencyId",
                schema: "fin",
                table: "VaultAssets",
                columns: new[] { "VaultId", "CurrencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_CreateUserId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_CurrencyId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_FinalConfirmedByUserId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "FinalConfirmedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_InitialConfirmedByUserId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "InitialConfirmedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_PaidByUserId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "PaidByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_PartyId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_PaymentDocumentId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "PaymentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_RequestNo",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "RequestNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_StatusId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_VaultId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_CashPaymentRequests_VaultMovementId",
                schema: "fin",
                table: "CashPaymentRequests",
                column: "VaultMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItemCustodyLogs_ActionDate",
                schema: "fin",
                table: "VaultItemCustodyLogs",
                column: "ActionDate");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItemCustodyLogs_ExecutedByUserId",
                schema: "fin",
                table: "VaultItemCustodyLogs",
                column: "ExecutedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItemCustodyLogs_HandedToUserId",
                schema: "fin",
                table: "VaultItemCustodyLogs",
                column: "HandedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItemCustodyLogs_VaultCustodyActionTypeId",
                schema: "fin",
                table: "VaultItemCustodyLogs",
                column: "VaultCustodyActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItemCustodyLogs_VaultItemId",
                schema: "fin",
                table: "VaultItemCustodyLogs",
                column: "VaultItemId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_CurrencyId",
                schema: "fin",
                table: "VaultItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_CurrentHolderUserId",
                schema: "fin",
                table: "VaultItems",
                column: "CurrentHolderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_ExpiryDate",
                schema: "fin",
                table: "VaultItems",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_VaultId",
                schema: "fin",
                table: "VaultItems",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_VaultItemCategoryId",
                schema: "fin",
                table: "VaultItems",
                column: "VaultItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultItems_VaultItemStatusId",
                schema: "fin",
                table: "VaultItems",
                column: "VaultItemStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaultAssets_Currency_CurrencyId",
                schema: "fin",
                table: "VaultAssets",
                column: "CurrencyId",
                principalSchema: "base",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VaultMovements_Currency_CurrencyId",
                schema: "fin",
                table: "VaultMovements",
                column: "CurrencyId",
                principalSchema: "base",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VaultMovements_InOut_InOutId",
                schema: "fin",
                table: "VaultMovements",
                column: "InOutId",
                principalSchema: "base",
                principalTable: "InOut",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VaultMovements_VaultMovementSourceTypes_VaultMovementSourceTypeId",
                schema: "fin",
                table: "VaultMovements",
                column: "VaultMovementSourceTypeId",
                principalSchema: "fin",
                principalTable: "VaultMovementSourceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vaults_Branches_BranchId",
                schema: "fin",
                table: "Vaults",
                column: "BranchId",
                principalSchema: "base",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vaults_Users_KeeperUserId",
                schema: "fin",
                table: "Vaults",
                column: "KeeperUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaultAssets_Currency_CurrencyId",
                schema: "fin",
                table: "VaultAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_VaultMovements_Currency_CurrencyId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_VaultMovements_InOut_InOutId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_VaultMovements_VaultMovementSourceTypes_VaultMovementSourceTypeId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaults_Branches_BranchId",
                schema: "fin",
                table: "Vaults");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaults_Users_KeeperUserId",
                schema: "fin",
                table: "Vaults");

            migrationBuilder.DropTable(
                name: "CashPaymentRequests",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "VaultItemCustodyLogs",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "VaultMovementSourceTypes",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "CashPaymentRequestStatus",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "VaultCustodyActionTypes",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "VaultItems",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "VaultItemCategory",
                schema: "fin");

            migrationBuilder.DropTable(
                name: "VaultItemStatus",
                schema: "fin");

            migrationBuilder.DropIndex(
                name: "IX_Vaults_BranchId",
                schema: "fin",
                table: "Vaults");

            migrationBuilder.DropIndex(
                name: "IX_Vaults_KeeperUserId",
                schema: "fin",
                table: "Vaults");

            migrationBuilder.DropIndex(
                name: "IX_VaultMovements_InOutId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropIndex(
                name: "IX_VaultMovements_MovementDate",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropIndex(
                name: "IX_VaultMovements_VaultId_CurrencyId",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropIndex(
                name: "IX_VaultAssets_VaultId_CurrencyId",
                schema: "fin",
                table: "VaultAssets");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "fin",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "fin",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "KeeperUserId",
                schema: "fin",
                table: "Vaults");

            migrationBuilder.DropColumn(
                name: "DepositorOrReceiver",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                schema: "fin",
                table: "VaultMovements");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                schema: "fin",
                table: "VaultAssets");

            migrationBuilder.RenameColumn(
                name: "VaultMovementSourceTypeId",
                schema: "fin",
                table: "VaultMovements",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "SourceEntityId",
                schema: "fin",
                table: "VaultMovements",
                newName: "VaultAssetId");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                schema: "fin",
                table: "VaultMovements",
                newName: "AssetTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_VaultMovements_VaultMovementSourceTypeId",
                schema: "fin",
                table: "VaultMovements",
                newName: "IX_VaultMovements_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_VaultMovements_CurrencyId",
                schema: "fin",
                table: "VaultMovements",
                newName: "IX_VaultMovements_AssetTypeId");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                schema: "fin",
                table: "VaultAssets",
                newName: "AssetTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_VaultAssets_CurrencyId",
                schema: "fin",
                table: "VaultAssets",
                newName: "IX_VaultAssets_AssetTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "VaultName",
                schema: "fin",
                table: "Vaults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                schema: "fin",
                table: "VaultMovements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuthorizeDate",
                schema: "fin",
                table: "VaultMovements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorizedByUserId",
                schema: "fin",
                table: "VaultMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstConfirmDate",
                schema: "fin",
                table: "VaultMovements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FirstConfirmedByUserId",
                schema: "fin",
                table: "VaultMovements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                schema: "fin",
                table: "VaultAssets",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetCategory",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetCategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SystemDefined = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultAcknowledges",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VaultMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ReceivedByName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultAcknowledges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultAcknowledges_Persons_ReceiverId",
                        column: x => x.ReceiverId,
                        principalSchema: "base",
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VaultAcknowledges_VaultMovements_VaultMovementId",
                        column: x => x.VaultMovementId,
                        principalSchema: "fin",
                        principalTable: "VaultMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaultMovementStatus",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StatusLocalName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultMovementStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                schema: "fin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetCategoryId = table.Column<int>(type: "int", nullable: false),
                    AssetTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SystemDefined = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetTypes_AssetCategory_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalSchema: "fin",
                        principalTable: "AssetCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "fin",
                table: "AssetCategory",
                columns: new[] { "Id", "AssetCategoryName", "IsActive", "IsDeleted", "SystemDefined" },
                values: new object[,]
                {
                    { 1, "Cash", true, false, true },
                    { 2, "Cheque", true, false, true },
                    { 3, "Document", true, false, true },
                    { 4, "Guarantee", true, false, true },
                    { 5, "Bonds", true, false, true }
                });

            migrationBuilder.InsertData(
                schema: "fin",
                table: "VaultMovementStatus",
                columns: new[] { "Id", "IsActive", "IsDeleted", "StatusLocalName", "StatusName" },
                values: new object[,]
                {
                    { 1, true, false, "در حال ثبت", "Draft" },
                    { 2, true, false, "منتظر مجوز", "Waiting For Authorize" },
                    { 3, true, false, "مجوز صادر شد", "Authorized" },
                    { 4, true, false, "انجام شد", "Executed" },
                    { 5, true, false, "رد شد", "Rejected" },
                    { 10, true, false, "لغو شد", "Canceled" }
                });

            migrationBuilder.InsertData(
                schema: "fin",
                table: "AssetTypes",
                columns: new[] { "Id", "AssetCategoryId", "AssetTypeName", "IsActive", "IsDeleted", "SystemDefined" },
                values: new object[,]
                {
                    { 1, 1, "US Dollar", true, false, true },
                    { 2, 1, "Euro", true, false, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VaultMovements_AuthorizedByUserId",
                schema: "fin",
                table: "VaultMovements",
                column: "AuthorizedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultMovements_VaultAssetId",
                schema: "fin",
                table: "VaultMovements",
                column: "VaultAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultMovements_VaultId",
                schema: "fin",
                table: "VaultMovements",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultAssets_VaultId",
                schema: "fin",
                table: "VaultAssets",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_AssetCategoryId",
                schema: "fin",
                table: "AssetTypes",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultAcknowledges_ReceiverId",
                schema: "fin",
                table: "VaultAcknowledges",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultAcknowledges_VaultMovementId",
                schema: "fin",
                table: "VaultAcknowledges",
                column: "VaultMovementId");

            migrationBuilder.AddForeignKey(
                name: "FK_VaultAssets_AssetTypes_AssetTypeId",
                schema: "fin",
                table: "VaultAssets",
                column: "AssetTypeId",
                principalSchema: "fin",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VaultMovements_AssetTypes_AssetTypeId",
                schema: "fin",
                table: "VaultMovements",
                column: "AssetTypeId",
                principalSchema: "fin",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VaultMovements_Users_AuthorizedByUserId",
                schema: "fin",
                table: "VaultMovements",
                column: "AuthorizedByUserId",
                principalSchema: "sec",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VaultMovements_VaultAssets_VaultAssetId",
                schema: "fin",
                table: "VaultMovements",
                column: "VaultAssetId",
                principalSchema: "fin",
                principalTable: "VaultAssets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VaultMovements_VaultMovementStatus_StatusId",
                schema: "fin",
                table: "VaultMovements",
                column: "StatusId",
                principalSchema: "fin",
                principalTable: "VaultMovementStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

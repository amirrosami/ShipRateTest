using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TwoFactorMethodId",
                schema: "sec",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TwoFactorMethods",
                schema: "sec",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TwoFactorMethodName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserOtpCodes",
                schema: "sec",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    LastAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_UserOtpCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOtpCodes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "sec",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "sec",
                table: "TwoFactorMethods",
                columns: new[] { "Id", "TwoFactorMethodName" },
                values: new object[,]
                {
                    { 1, "None" },
                    { 2, "Sms" },
                    { 3, "GoogleAuthenticator" },
                    { 4, "Email" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TwoFactorMethodId",
                schema: "sec",
                table: "Users",
                column: "TwoFactorMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOtpCodes_UserId_Method_IsUsed_ExpiresAt",
                schema: "sec",
                table: "UserOtpCodes",
                columns: new[] { "UserId", "Method", "IsUsed", "ExpiresAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TwoFactorMethods_TwoFactorMethodId",
                schema: "sec",
                table: "Users",
                column: "TwoFactorMethodId",
                principalSchema: "sec",
                principalTable: "TwoFactorMethods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_TwoFactorMethods_TwoFactorMethodId",
                schema: "sec",
                table: "Users");

            migrationBuilder.DropTable(
                name: "TwoFactorMethods",
                schema: "sec");

            migrationBuilder.DropTable(
                name: "UserOtpCodes",
                schema: "sec");

            migrationBuilder.DropIndex(
                name: "IX_Users_TwoFactorMethodId",
                schema: "sec",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwoFactorMethodId",
                schema: "sec",
                table: "Users");
        }
    }
}

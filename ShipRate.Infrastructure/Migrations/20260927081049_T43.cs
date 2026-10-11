using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T43 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketingCallLogs",
                schema: "comms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InquiryReference = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CallStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CallEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Outcome = table.Column<byte>(type: "tinyint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CallerUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CallerExtension = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    PhoneIpAddress = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_MarketingCallLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCallLogs_CallerUserId",
                schema: "comms",
                table: "MarketingCallLogs",
                column: "CallerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCallLogs_CallStartTime",
                schema: "comms",
                table: "MarketingCallLogs",
                column: "CallStartTime");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCallLogs_FollowUpDate",
                schema: "comms",
                table: "MarketingCallLogs",
                column: "FollowUpDate");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCallLogs_InquiryReference",
                schema: "comms",
                table: "MarketingCallLogs",
                column: "InquiryReference");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCallLogs_PhoneNumber",
                schema: "comms",
                table: "MarketingCallLogs",
                column: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketingCallLogs",
                schema: "comms");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipRate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ابتدا FK/PK محدودیت‌ها را حذف کن (PK باید drop شود)
            migrationBuilder.DropPrimaryKey(
                name: "PK_SmsIncomingMessages",
                schema: "comms",
                table: "SmsIncomingMessages");

            // ستون Identity حذف شود
            migrationBuilder.DropColumn(
                name: "Id",
                schema: "comms",
                table: "SmsIncomingMessages");

            // ستون جدید بدون Identity اضافه شود
            migrationBuilder.AddColumn<long>(
                name: "Id",
                schema: "comms",
                table: "SmsIncomingMessages",
                type: "bigint",
                nullable: false);

            // PK دوباره ساخته شود
            migrationBuilder.AddPrimaryKey(
                name: "PK_SmsIncomingMessages",
                schema: "comms",
                table: "SmsIncomingMessages",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SmsIncomingMessages",
                schema: "comms",
                table: "SmsIncomingMessages");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "comms",
                table: "SmsIncomingMessages");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                schema: "comms",
                table: "SmsIncomingMessages",
                type: "bigint",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SmsIncomingMessages",
                schema: "comms",
                table: "SmsIncomingMessages",
                column: "Id");
        }
    }
}

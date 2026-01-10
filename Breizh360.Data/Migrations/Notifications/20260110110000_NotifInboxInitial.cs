using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Breizh360.Data.Migrations.Notifications;

/// <summary>
/// Migration initiale Notifications Inbox.
/// </summary>
public partial class NotifInboxInitial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "notif_inbox_notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Payload = table.Column<string>(type: "character varying(32000)", maxLength: 32000, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                ReadAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                SentAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                NextAttemptAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IdempotencyKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_notif_inbox_notifications", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_notif_inbox_notifications_UserId_CreatedAtUtc",
            table: "notif_inbox_notifications",
            columns: new[] { "UserId", "CreatedAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_notif_inbox_notifications_UserId_IsRead",
            table: "notif_inbox_notifications",
            columns: new[] { "UserId", "IsRead" });

        migrationBuilder.CreateIndex(
            name: "IX_notif_inbox_notifications_Status_NextAttemptAtUtc",
            table: "notif_inbox_notifications",
            columns: new[] { "Status", "NextAttemptAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_notif_inbox_notifications_UserId_IdempotencyKey",
            table: "notif_inbox_notifications",
            columns: new[] { "UserId", "IdempotencyKey" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "notif_inbox_notifications");
    }
}

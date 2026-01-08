using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Breizh360.Data.Migrations.Auth;

/// <summary>
/// Migration initiale Auth (Users/Roles/Permissions + liens + RefreshTokens).
/// </summary>
public partial class AuthInitial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "auth_permissions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_auth_permissions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "auth_roles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_auth_roles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "auth_users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Login = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_auth_users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "auth_role_permissions",
            columns: table => new
            {
                RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_auth_role_permissions", x => new { x.RoleId, x.PermissionId });
                table.ForeignKey(
                    name: "FK_auth_role_permissions_auth_permissions_PermissionId",
                    column: x => x.PermissionId,
                    principalTable: "auth_permissions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_auth_role_permissions_auth_roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "auth_roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "auth_user_roles",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                RoleId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_auth_user_roles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_auth_user_roles_auth_roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "auth_roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_auth_user_roles_auth_users_UserId",
                    column: x => x.UserId,
                    principalTable: "auth_users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "auth_refresh_tokens",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                TokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                ReplacedByTokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                RevokedReason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_auth_refresh_tokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_auth_refresh_tokens_auth_users_UserId",
                    column: x => x.UserId,
                    principalTable: "auth_users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_auth_permissions_Code",
            table: "auth_permissions",
            column: "Code",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_auth_users_Login",
            table: "auth_users",
            column: "Login",
            unique: true);


        migrationBuilder.CreateIndex(
            name: "IX_auth_roles_Name",
            table: "auth_roles",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_auth_users_Email",
            table: "auth_users",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_auth_refresh_tokens_TokenHash",
            table: "auth_refresh_tokens",
            column: "TokenHash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_auth_refresh_tokens_UserId",
            table: "auth_refresh_tokens",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_auth_role_permissions_PermissionId",
            table: "auth_role_permissions",
            column: "PermissionId");

        migrationBuilder.CreateIndex(
            name: "IX_auth_user_roles_RoleId",
            table: "auth_user_roles",
            column: "RoleId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "auth_refresh_tokens");
        migrationBuilder.DropTable(name: "auth_role_permissions");
        migrationBuilder.DropTable(name: "auth_user_roles");
        migrationBuilder.DropTable(name: "auth_permissions");
        migrationBuilder.DropTable(name: "auth_roles");
        migrationBuilder.DropTable(name: "auth_users");
    }
}

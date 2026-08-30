using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplePos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleClosingAndRenameUserPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_permissions_PermissionId",
                table: "UserPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_users_UserId",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissions",
                table: "UserPermissions");

            migrationBuilder.RenameTable(
                name: "UserPermissions",
                newName: "user_permissions");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "user_permissions",
                newName: "IX_user_permissions_PermissionId");

            migrationBuilder.AddColumn<bool>(
                name: "Closed",
                table: "sales",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ClosedByUserId",
                table: "sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeClosed",
                table: "sales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_permissions",
                table: "user_permissions",
                columns: new[] { "UserId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_sales_ClosedByUserId",
                table: "sales",
                column: "ClosedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sales_users_ClosedByUserId",
                table: "sales",
                column: "ClosedByUserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_permissions_permissions_PermissionId",
                table: "user_permissions",
                column: "PermissionId",
                principalTable: "permissions",
                principalColumn: "PermissionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_permissions_users_UserId",
                table: "user_permissions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sales_users_ClosedByUserId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_permissions_PermissionId",
                table: "user_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_permissions_users_UserId",
                table: "user_permissions");

            migrationBuilder.DropIndex(
                name: "IX_sales_ClosedByUserId",
                table: "sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_permissions",
                table: "user_permissions");

            migrationBuilder.DropColumn(
                name: "Closed",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "ClosedByUserId",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "DateTimeClosed",
                table: "sales");

            migrationBuilder.RenameTable(
                name: "user_permissions",
                newName: "UserPermissions");

            migrationBuilder.RenameIndex(
                name: "IX_user_permissions_PermissionId",
                table: "UserPermissions",
                newName: "IX_UserPermissions_PermissionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissions",
                table: "UserPermissions",
                columns: new[] { "UserId", "PermissionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_permissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId",
                principalTable: "permissions",
                principalColumn: "PermissionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_users_UserId",
                table: "UserPermissions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

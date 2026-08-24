using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplePos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "sales",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeVoided",
                table: "sales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VoidedByUserId",
                table: "sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedByUserId",
                table: "sale_payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AddedByUserId",
                table: "sale_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeAdded",
                table: "sale_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeVoided",
                table: "sale_items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Void",
                table: "sale_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "VoidedByUserId",
                table: "sale_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sales_CreatedByUserId",
                table: "sales",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_VoidedByUserId",
                table: "sales",
                column: "VoidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sale_payments_ProcessedByUserId",
                table: "sale_payments",
                column: "ProcessedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sale_items_AddedByUserId",
                table: "sale_items",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sale_items_VoidedByUserId",
                table: "sale_items",
                column: "VoidedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sale_items_users_AddedByUserId",
                table: "sale_items",
                column: "AddedByUserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sale_items_users_VoidedByUserId",
                table: "sale_items",
                column: "VoidedByUserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sale_payments_users_ProcessedByUserId",
                table: "sale_payments",
                column: "ProcessedByUserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_users_CreatedByUserId",
                table: "sales",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_users_VoidedByUserId",
                table: "sales",
                column: "VoidedByUserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sale_items_users_AddedByUserId",
                table: "sale_items");

            migrationBuilder.DropForeignKey(
                name: "FK_sale_items_users_VoidedByUserId",
                table: "sale_items");

            migrationBuilder.DropForeignKey(
                name: "FK_sale_payments_users_ProcessedByUserId",
                table: "sale_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_users_CreatedByUserId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_users_VoidedByUserId",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sales_CreatedByUserId",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sales_VoidedByUserId",
                table: "sales");

            migrationBuilder.DropIndex(
                name: "IX_sale_payments_ProcessedByUserId",
                table: "sale_payments");

            migrationBuilder.DropIndex(
                name: "IX_sale_items_AddedByUserId",
                table: "sale_items");

            migrationBuilder.DropIndex(
                name: "IX_sale_items_VoidedByUserId",
                table: "sale_items");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "DateTimeVoided",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "VoidedByUserId",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "ProcessedByUserId",
                table: "sale_payments");

            migrationBuilder.DropColumn(
                name: "AddedByUserId",
                table: "sale_items");

            migrationBuilder.DropColumn(
                name: "DateTimeAdded",
                table: "sale_items");

            migrationBuilder.DropColumn(
                name: "DateTimeVoided",
                table: "sale_items");

            migrationBuilder.DropColumn(
                name: "Void",
                table: "sale_items");

            migrationBuilder.DropColumn(
                name: "VoidedByUserId",
                table: "sale_items");
        }
    }
}

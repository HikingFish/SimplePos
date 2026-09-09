using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplePos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductOutletAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outlet_product_availabilities",
                columns: table => new
                {
                    OutletId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    MarkedUnavailableAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MarkedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outlet_product_availabilities", x => new { x.OutletId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_outlet_product_availabilities_outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "outlets",
                        principalColumn: "OutletId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_outlet_product_availabilities_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_outlet_product_availabilities_users_MarkedByUserId",
                        column: x => x.MarkedByUserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_outlet_accesses",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OutletId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_outlet_accesses", x => new { x.UserId, x.OutletId });
                    table.ForeignKey(
                        name: "FK_user_outlet_accesses_outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "outlets",
                        principalColumn: "OutletId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_outlet_accesses_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_outlet_product_availabilities_MarkedByUserId",
                table: "outlet_product_availabilities",
                column: "MarkedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_outlet_product_availabilities_ProductId",
                table: "outlet_product_availabilities",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_user_outlet_accesses_OutletId",
                table: "user_outlet_accesses",
                column: "OutletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outlet_product_availabilities");

            migrationBuilder.DropTable(
                name: "user_outlet_accesses");
        }
    }
}

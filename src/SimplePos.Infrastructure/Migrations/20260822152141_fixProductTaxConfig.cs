using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplePos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixProductTaxConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_taxes_products_TaxId",
                table: "product_taxes");

            migrationBuilder.CreateIndex(
                name: "IX_product_taxes_ProductId",
                table: "product_taxes",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_product_taxes_products_ProductId",
                table: "product_taxes",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_taxes_products_ProductId",
                table: "product_taxes");

            migrationBuilder.DropIndex(
                name: "IX_product_taxes_ProductId",
                table: "product_taxes");

            migrationBuilder.AddForeignKey(
                name: "FK_product_taxes_products_TaxId",
                table: "product_taxes",
                column: "TaxId",
                principalTable: "products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

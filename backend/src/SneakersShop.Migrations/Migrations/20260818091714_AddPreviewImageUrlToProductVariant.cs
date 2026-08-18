using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakersShop.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPreviewImageUrlToProductVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreviewImageUrl",
                table: "ProductVariants",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviewImageUrl",
                table: "ProductVariants");
        }
    }
}
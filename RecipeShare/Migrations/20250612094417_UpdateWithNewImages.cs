using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeShare.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWithNewImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImageUrl",
                value: "https://www.themealdb.com/images/media/meals/wyxwsp1486979827.jpg");

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImageUrl",
                value: "https://www.themealdb.com/images/media/meals/tvtxpq1511464705.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImageUrl",
                value: "https://www.themealdb.com/images/media/meals/wxu3ux1487348960.jpg");

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImageUrl",
                value: "https://www.themealdb.com/images/media/meals/ysxwrt1511817232.jpg");
        }
    }
}

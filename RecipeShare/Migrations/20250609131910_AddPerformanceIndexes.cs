using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeShare.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recipes_PrepTimeMinutes_CookTimeMinutes",
                table: "Recipes",
                columns: new[] { "PrepTimeMinutes", "CookTimeMinutes" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_PrepTimeMinutes_CookTimeMinutes",
                table: "Recipes");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DietaryTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DifficultyLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DifficultyLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrepTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    CookTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    Servings = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DifficultyLevelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_DifficultyLevels_DifficultyLevelId",
                        column: x => x.DifficultyLevelId,
                        principalTable: "DifficultyLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeDietaryTags",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    DietaryTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeDietaryTags", x => new { x.RecipeId, x.DietaryTagId });
                    table.ForeignKey(
                        name: "FK_RecipeDietaryTags_DietaryTags_DietaryTagId",
                        column: x => x.DietaryTagId,
                        principalTable: "DietaryTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeDietaryTags_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DietaryTags",
                columns: new[] { "Id", "Description", "DisplayName", "Name" },
                values: new object[,]
                {
                    { 1, "Contains no meat or fish", "Vegetarian", "Vegetarian" },
                    { 2, "Contains no animal products", "Vegan", "Vegan" },
                    { 3, "Contains no gluten", "Gluten-Free", "GlutenFree" },
                    { 4, "Contains no dairy products", "Dairy-Free", "DairyFree" },
                    { 5, "High in protein content", "High Protein", "HighProtein" },
                    { 6, "Low in carbohydrates", "Low-Carb", "LowCarb" },
                    { 7, "Contains no nuts", "Nut-Free", "NutFree" }
                });

            migrationBuilder.InsertData(
                table: "DifficultyLevels",
                columns: new[] { "Id", "Description", "DisplayName", "Name" },
                values: new object[,]
                {
                    { 1, "Simple recipes with basic techniques and few ingredients", "Beginner (Easy)", "Beginner" },
                    { 2, "Recipes requiring some cooking experience and moderate techniques", "Intermediate (Medium)", "Intermediate" },
                    { 3, "Complex recipes requiring advanced techniques and experience", "Advanced (Hard)", "Advanced" }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "CookTimeMinutes", "CreatedAt", "Description", "DifficultyLevelId", "ImageUrl", "Instructions", "PrepTimeMinutes", "Servings", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A simple and delicious traditional Italian pizza", 1, "https://www.themealdb.com/images/media/meals/x0lk931587671540.jpg", "1. Preheat oven to 450°F\n2. Roll out pizza dough\n3. Add sauce and toppings\n4. Bake for 12-15 minutes", 20, 4, "Classic Margherita Pizza", null },
                    { 2, 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quick and healthy Asian-inspired dish", 2, "https://www.themealdb.com/images/media/meals/1520084413.jpg", "1. Cut chicken into strips\n2. Stir fry vegetables\n3. Add chicken and sauce\n4. Serve over rice", 15, 4, "Chicken Stir Fry", null },
                    { 3, 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classic homemade cookies", 1, "https://www.themealdb.com/images/media/meals/wyrqqq1468233628.jpg", "1. Mix dry ingredients\n2. Cream butter and sugar\n3. Add chocolate chips\n4. Bake at 350°F for 10-12 minutes", 15, 24, "Chocolate Chip Cookies", null }
                });

            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "Id", "Amount", "Name", "RecipeId", "Unit" },
                values: new object[,]
                {
                    { 1, "1", "Pizza Dough", 1, "ball" },
                    { 2, "1/2", "Tomato Sauce", 1, "cup" },
                    { 3, "8", "Fresh Mozzarella", 1, "oz" },
                    { 4, "1/4", "Fresh Basil", 1, "cup" },
                    { 5, "2", "Olive Oil", 1, "tbsp" },
                    { 6, "1", "Chicken Breast", 2, "lb" },
                    { 7, "2", "Broccoli", 2, "cups" },
                    { 8, "2", "Bell Peppers", 2, "medium" },
                    { 9, "1/4", "Soy Sauce", 2, "cup" },
                    { 10, "1", "Ginger", 2, "tbsp" },
                    { 11, "2", "All-Purpose Flour", 3, "cups" },
                    { 12, "1", "Butter", 3, "cup" },
                    { 13, "3/4", "Brown Sugar", 3, "cup" },
                    { 14, "2", "Chocolate Chips", 3, "cups" },
                    { 15, "1", "Vanilla Extract", 3, "tsp" }
                });

            migrationBuilder.InsertData(
                table: "RecipeDietaryTags",
                columns: new[] { "DietaryTagId", "RecipeId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 4, 1 },
                    { 5, 2 },
                    { 1, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DietaryTags_Name",
                table: "DietaryTags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevels_Name",
                table: "DifficultyLevels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Name",
                table: "Ingredients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_RecipeId",
                table: "Ingredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDietaryTags_DietaryTagId",
                table: "RecipeDietaryTags",
                column: "DietaryTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CreatedAt",
                table: "Recipes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_DifficultyLevelId",
                table: "Recipes",
                column: "DifficultyLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Title",
                table: "Recipes",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "RecipeDietaryTags");

            migrationBuilder.DropTable(
                name: "DietaryTags");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "DifficultyLevels");
        }
    }
}

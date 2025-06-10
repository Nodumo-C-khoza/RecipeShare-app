using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreRecipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "Id", "Amount", "Name", "RecipeId", "Unit" },
                values: new object[,]
                {
                    { 66, "2", "Almond Flour", 7, "cups" },
                    { 67, "3/4", "Cocoa Powder", 7, "cup" },
                    { 68, "1", "Coconut Sugar", 7, "cup" },
                    { 69, "1/2", "Coconut Oil", 7, "cup" },
                    { 70, "1", "Almond Milk", 7, "cup" }
                });

            migrationBuilder.InsertData(
                table: "RecipeDietaryTags",
                columns: new[] { "DietaryTagId", "RecipeId" },
                values: new object[,]
                {
                    { 3, 7 },
                    { 4, 7 }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "CookTimeMinutes", "CreatedAt", "Description", "DifficultyLevelId", "ImageUrl", "Instructions", "PrepTimeMinutes", "Servings", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, 30, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A nourishing bowl packed with vegetables and protein", 1, "https://www.themealdb.com/images/media/meals/1550441882.jpg", "1. Cook quinoa according to package instructions\n2. Roast vegetables with olive oil and seasonings\n3. Prepare tahini dressing\n4. Assemble bowl with all components", 20, 2, "Vegetarian Buddha Bowl", null },
                    { 5, 40, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classic British dish with beef fillet wrapped in puff pastry", 3, "https://www.themealdb.com/images/media/meals/vvusxs1483907034.jpg", "1. Sear beef fillet\n2. Prepare mushroom duxelles\n3. Wrap in prosciutto and puff pastry\n4. Bake until golden brown", 45, 4, "Beef Wellington", null },
                    { 6, 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Plant-based version of the classic Thai noodle dish", 2, "https://www.themealdb.com/images/media/meals/vtxyxv1483567157.jpg", "1. Soak rice noodles\n2. Prepare tofu and vegetables\n3. Make pad thai sauce\n4. Stir fry all components together", 25, 4, "Vegan Pad Thai", null },
                    { 8, 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Healthy salmon with Mediterranean flavors", 2, "https://www.themealdb.com/images/media/meals/1550441275.jpg", "1. Marinate salmon\n2. Prepare vegetable medley\n3. Grill salmon\n4. Serve with vegetables", 15, 4, "Mediterranean Grilled Salmon", null },
                    { 9, 25, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Protein-packed bowl with quinoa and vegetables", 1, "https://www.themealdb.com/images/media/meals/1550441882.jpg", "1. Cook quinoa\n2. Roast vegetables\n3. Prepare tahini dressing\n4. Assemble bowl", 20, 2, "Quinoa Buddha Bowl", null },
                    { 10, 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Healthy alternative to traditional rice", 1, "https://www.themealdb.com/images/media/meals/1550441882.jpg", "1. Process cauliflower\n2. Cook with seasonings\n3. Add vegetables\n4. Serve hot", 10, 4, "Low-Carb Cauliflower Rice", null },
                    { 11, 35, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rich and moist chocolate cake without gluten", 2, "https://www.themealdb.com/images/media/meals/1550441275.jpg", "1. Mix dry ingredients\n2. Combine wet ingredients\n3. Bake at 350°F\n4. Prepare chocolate ganache", 20, 12, "Gluten-Free Chocolate Cake", null }
                });

            migrationBuilder.InsertData(
                table: "Ingredients",
                columns: new[] { "Id", "Amount", "Name", "RecipeId", "Unit" },
                values: new object[,]
                {
                    { 51, "1", "Quinoa", 4, "cup" },
                    { 52, "1", "Sweet Potato", 4, "medium" },
                    { 53, "1", "Chickpeas", 4, "can" },
                    { 54, "2", "Kale", 4, "cups" },
                    { 55, "2", "Tahini", 4, "tbsp" },
                    { 56, "2", "Beef Fillet", 5, "lbs" },
                    { 57, "1", "Mushrooms", 5, "lb" },
                    { 58, "1", "Puff Pastry", 5, "sheet" },
                    { 59, "8", "Prosciutto", 5, "slices" },
                    { 60, "2", "Dijon Mustard", 5, "tbsp" },
                    { 61, "8", "Rice Noodles", 6, "oz" },
                    { 62, "14", "Tofu", 6, "oz" },
                    { 63, "2", "Bean Sprouts", 6, "cups" },
                    { 64, "1/2", "Peanuts", 6, "cup" },
                    { 65, "2", "Tamarind Paste", 6, "tbsp" },
                    { 71, "4", "Salmon Fillets", 8, "pieces" },
                    { 72, "1", "Lemon", 8, "whole" },
                    { 73, "2", "Olive Oil", 8, "tbsp" },
                    { 74, "4", "Garlic", 8, "cloves" },
                    { 75, "1/4", "Herbs", 8, "cup" },
                    { 76, "1", "Quinoa", 9, "cup" },
                    { 77, "1", "Avocado", 9, "whole" },
                    { 78, "1", "Black Beans", 9, "can" },
                    { 79, "1", "Corn", 9, "cup" },
                    { 80, "1", "Lime", 9, "whole" },
                    { 81, "1", "Cauliflower", 10, "head" },
                    { 82, "1", "Onion", 10, "medium" },
                    { 83, "2", "Garlic", 10, "cloves" },
                    { 84, "2", "Olive Oil", 10, "tbsp" },
                    { 85, "1/4", "Herbs", 10, "cup" }
                });

            migrationBuilder.InsertData(
                table: "RecipeDietaryTags",
                columns: new[] { "DietaryTagId", "RecipeId" },
                values: new object[,]
                {
                    { 1, 4 },
                    { 2, 4 },
                    { 3, 4 },
                    { 5, 5 },
                    { 2, 6 },
                    { 3, 6 },
                    { 5, 8 },
                    { 6, 8 },
                    { 1, 9 },
                    { 2, 9 },
                    { 3, 9 },
                    { 1, 10 },
                    { 6, 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Ingredients",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 2, 6 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 3, 6 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 3, 7 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 4, 7 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 5, 8 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 6, 8 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 1, 9 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 2, 9 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 3, 9 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 1, 10 });

            migrationBuilder.DeleteData(
                table: "RecipeDietaryTags",
                keyColumns: new[] { "DietaryTagId", "RecipeId" },
                keyValues: new object[] { 6, 10 });

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}

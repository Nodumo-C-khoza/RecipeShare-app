using RecipeShare.Data;

namespace RecipeShare.Interfaces
{
    public interface IRecipeRepository
    {
        Task<(IEnumerable<Recipe> Recipes, int TotalCount)> GetAllRecipesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchQuery = null,
            string? tag = null,
            string? difficulty = null,
            int? maxTime = null,
            bool quickRecipes = false
        );
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<IEnumerable<Recipe>> GetRecipesByDietaryTagAsync(string tag);
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task<Recipe> UpdateRecipeAsync(Recipe recipe);
        Task<bool> DeleteRecipeAsync(int id);
        Task<IEnumerable<Recipe>> GetRecipesByTotalTimeAsync(int maxMinutes);
        Task<IEnumerable<Recipe>> GetRecipesByDifficultyLevelAsync(string difficultyLevel);
        Task<IEnumerable<Recipe>> GetQuickRecipesAsync(int maxMinutes = 30);
        Task<IEnumerable<Recipe>> GetRecipesByIngredientsAsync(
            IEnumerable<string> ingredients,
            bool matchAll = false
        );
        Task<IEnumerable<DietaryTag>> GetAvailableDietaryTagsAsync();
        Task<IEnumerable<DifficultyLevel>> GetAvailableDifficultyLevelsAsync();
    }
}

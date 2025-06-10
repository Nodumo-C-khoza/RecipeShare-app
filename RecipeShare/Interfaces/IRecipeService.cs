using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeShare.Data;
using RecipeShare.Models;

namespace RecipeShare.Interfaces
{
    public interface IRecipeService
    {
        Task<(IEnumerable<RecipeListViewModel> Recipes, int TotalCount)> GetAllRecipesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchQuery = null,
            string? tag = null,
            string? difficulty = null,
            int? maxTime = null,
            bool quickRecipes = false
        );
        Task<RecipeDetailViewModel> GetRecipeByIdAsync(int id);
        Task<IEnumerable<RecipeListViewModel>> GetRecipesByDietaryTagAsync(string tag);
        Task<RecipeDetailViewModel> CreateRecipeAsync(CreateRecipeViewModel recipe);
        Task<RecipeDetailViewModel> UpdateRecipeAsync(int id, UpdateRecipeViewModel recipe);
        Task<bool> DeleteRecipeAsync(int id);
        Task<IEnumerable<RecipeListViewModel>> GetRecipesByTotalTimeAsync(int maxMinutes);
        Task<IEnumerable<RecipeListViewModel>> GetRecipesByDifficultyLevelAsync(
            string difficultyLevel
        );
        Task<IEnumerable<RecipeListViewModel>> GetQuickRecipesAsync(int maxMinutes = 30);
        Task<IEnumerable<RecipeListViewModel>> GetRecipesByIngredientsAsync(
            IEnumerable<string> ingredients,
            bool matchAll = false
        );
        Task<IEnumerable<DietaryTag>> GetAvailableDietaryTagsAsync();
        Task<IEnumerable<string>> GetAvailableDifficultyLevelsAsync();
    }
}

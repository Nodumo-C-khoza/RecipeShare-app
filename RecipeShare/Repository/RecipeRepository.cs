using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Interfaces;
using RecipeShare.Models;

namespace RecipeShare.Repository
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IEasyCachingProvider _cache;
        private const int DefaultPageSize = 20;
        private const string RecipeListCacheKey = "recipe:list:{0}:{1}"; // {pageNumber}_{pageSize}
        private const string RecipeByIdCacheKey = "recipe:{0}"; // {id}
        private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(2);

        public RecipeRepository(ApplicationDbContext context, IEasyCachingProvider cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<(IEnumerable<Recipe> Recipes, int TotalCount)> GetAllRecipesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchQuery = null,
            string? tag = null,
            string? difficulty = null,
            int? maxTime = null,
            bool quickRecipes = false
        )
        {
            var cacheKey = string.Format(RecipeListCacheKey, pageNumber, pageSize);

            var cacheValue = await _cache.GetAsync<(IEnumerable<Recipe>, int)>(cacheKey);
            if (cacheValue.HasValue)
            {
                return cacheValue.Value;
            }

            var query = _context
                .Recipes.Include(r => r.Ingredients)
                .Include(r => r.DifficultyLevel)
                .Include(r => r.DietaryTags)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(r =>
                    r.Title.Contains(searchQuery) || r.Description.Contains(searchQuery)
                );
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                query = query.Where(r => r.DietaryTags.Any(dt => dt.Name == tag));
            }

            if (!string.IsNullOrWhiteSpace(difficulty))
            {
                query = query.Where(r => r.DifficultyLevel.Name == difficulty);
            }

            if (maxTime.HasValue)
            {
                query = query.Where(r => (r.PrepTimeMinutes + r.CookTimeMinutes) <= maxTime.Value);
            }

            if (quickRecipes)
            {
                query = query.Where(r => (r.PrepTimeMinutes + r.CookTimeMinutes) <= 30);
            }

            var totalCount = await query.CountAsync();

            var recipes = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = (recipes, totalCount);
            await _cache.SetAsync(cacheKey, result, DefaultCacheDuration);

            return result;
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            var cacheKey = string.Format(RecipeByIdCacheKey, id);

            var cacheValue = await _cache.GetAsync<Recipe>(cacheKey);
            if (cacheValue.HasValue)
            {
                return cacheValue.Value;
            }

            var recipe = await _context
                .Recipes.AsNoTracking()
                .Include(r => r.Ingredients)
                .Include(r => r.DifficultyLevel)
                .Include(r => r.DietaryTags)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe != null)
            {
                await _cache.SetAsync(cacheKey, recipe, DefaultCacheDuration);
            }

            return recipe;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByDietaryTagAsync(string tag)
        {
            return await _context
                .Recipes.Include(r => r.Ingredients)
                .Include(r => r.DifficultyLevel)
                .Include(r => r.DietaryTags)
                .Where(r => r.DietaryTags.Any(dt => dt.Name == tag))
                .ToListAsync();
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            recipe.CreatedAt = DateTime.UtcNow;
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            await _cache.RemoveByPrefixAsync("recipe:list:");

            return recipe;
        }

        public async Task<Recipe> UpdateRecipeAsync(Recipe recipe)
        {
            var existingRecipe = await _context
                .Recipes.Include(r => r.Ingredients)
                .Include(r => r.DifficultyLevel)
                .Include(r => r.DietaryTags)
                .FirstOrDefaultAsync(r => r.Id == recipe.Id);

            if (existingRecipe == null)
                return null;

            // Update basic properties
            existingRecipe.Title = recipe.Title;
            existingRecipe.Description = recipe.Description;
            existingRecipe.Instructions = recipe.Instructions;
            existingRecipe.PrepTimeMinutes = recipe.PrepTimeMinutes;
            existingRecipe.CookTimeMinutes = recipe.CookTimeMinutes;
            existingRecipe.Servings = recipe.Servings;
            existingRecipe.ImageUrl = recipe.ImageUrl;
            existingRecipe.DifficultyLevelId = recipe.DifficultyLevelId;
            existingRecipe.UpdatedAt = DateTime.UtcNow;

            existingRecipe.DietaryTags.Clear();
            var dietaryTags = await _context
                .DietaryTags.Where(dt => recipe.DietaryTags.Select(t => t.Id).Contains(dt.Id))
                .ToListAsync();
            existingRecipe.DietaryTags = dietaryTags;

            _context.Ingredients.RemoveRange(existingRecipe.Ingredients);
            existingRecipe.Ingredients = recipe.Ingredients;

            await _context.SaveChangesAsync();

            await _cache.RemoveAsync(string.Format(RecipeByIdCacheKey, recipe.Id));
            await _cache.RemoveByPrefixAsync("recipe:list:");

            return existingRecipe;
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return false;

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            await _cache.RemoveAsync(string.Format(RecipeByIdCacheKey, id));
            await _cache.RemoveByPrefixAsync("recipe:list:");

            return true;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByTotalTimeAsync(int maxMinutes)
        {
            return await _context
                .Recipes.Include(r => r.Ingredients)
                .Include(r => r.DifficultyLevel)
                .Include(r => r.DietaryTags)
                .Where(r => r.PrepTimeMinutes + r.CookTimeMinutes <= maxMinutes)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByDifficultyLevelAsync(
            string difficultyLevel
        )
        {
            return await _context
                .Recipes.Include(r => r.Ingredients)
                .Include(r => r.DifficultyLevel)
                .Include(r => r.DietaryTags)
                .Where(r => r.DifficultyLevel.Name == difficultyLevel)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetQuickRecipesAsync(int maxMinutes = 30)
        {
            return await _context
                .Recipes.Include(r => r.Ingredients)
                .Include(r => r.DifficultyLevel)
                .Include(r => r.DietaryTags)
                .Where(r => r.PrepTimeMinutes + r.CookTimeMinutes <= maxMinutes)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByIngredientsAsync(
            IEnumerable<string> ingredients,
            bool matchAll = false
        )
        {
            var normalizedIngredients = ingredients.Select(i => i.ToLower().Trim()).ToList();

            var query = _context
                .Recipes.Include(r => r.Ingredients)
                .Include(r => r.DifficultyLevel)
                .Include(r => r.DietaryTags)
                .AsQueryable();

            if (matchAll)
            {
                foreach (var ingredient in normalizedIngredients)
                {
                    query = query.Where(r =>
                        r.Ingredients.Any(i => i.Name.ToLower().Contains(ingredient))
                    );
                }
            }
            else
            {
                query = query.Where(r =>
                    r.Ingredients.Any(i =>
                        normalizedIngredients.Any(searchIngredient =>
                            i.Name.ToLower().Contains(searchIngredient)
                        )
                    )
                );
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<DietaryTag>> GetAvailableDietaryTagsAsync()
        {
            return await _context.DietaryTags.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<IEnumerable<DifficultyLevel>> GetAvailableDifficultyLevelsAsync()
        {
            return await _context.DifficultyLevels.OrderBy(l => l.Name).ToListAsync();
        }
    }
}

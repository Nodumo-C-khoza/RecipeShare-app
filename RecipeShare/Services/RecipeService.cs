using RecipeShare.Data;
using RecipeShare.Interfaces;
using RecipeShare.Models;

namespace RecipeShare.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<(
            IEnumerable<RecipeListViewModel> Recipes,
            int TotalCount
        )> GetAllRecipesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchQuery = null,
            string? tag = null,
            string? difficulty = null,
            int? maxTime = null,
            bool quickRecipes = false
        )
        {
            var (recipes, totalCount) = await _recipeRepository.GetAllRecipesAsync(
                pageNumber,
                pageSize,
                searchQuery,
                tag,
                difficulty,
                maxTime,
                quickRecipes
            );
            return (recipes.Select(MapToRecipeListViewModel), totalCount);
        }

        public async Task<RecipeDetailViewModel> GetRecipeByIdAsync(int id)
        {
            var recipe = await _recipeRepository.GetRecipeByIdAsync(id);
            if (recipe == null)
                throw new KeyNotFoundException($"Recipe with ID {id} not found.");

            return MapToRecipeDetailViewModel(recipe);
        }

        public async Task<IEnumerable<RecipeListViewModel>> GetRecipesByDietaryTagAsync(string tag)
        {
            var recipes = await _recipeRepository.GetRecipesByDietaryTagAsync(tag);
            return recipes.Select(MapToRecipeListViewModel);
        }

        public async Task<RecipeDetailViewModel> CreateRecipeAsync(CreateRecipeViewModel viewModel)
        {
            if (viewModel.PrepTimeMinutes + viewModel.CookTimeMinutes > 480)
            {
                throw new ArgumentException("Total cooking time cannot exceed 8 hours");
            }

            // Validate difficulty level ID
            if (!viewModel.DifficultyLevelId.HasValue)
            {
                throw new ArgumentException("Difficulty level ID is required");
            }

            // Validate dietary tag IDs
            var availableTags = await _recipeRepository.GetAvailableDietaryTagsAsync();
            var validTagIds = new HashSet<int>(availableTags.Select(t => t.Id));
            if (
                viewModel.DietaryTagIds != null
                && viewModel.DietaryTagIds.Any(id => !validTagIds.Contains(id))
            )
            {
                throw new ArgumentException("Invalid dietary tag ID provided");
            }

            // Validate that the difficulty level exists
            var difficultyLevels = await _recipeRepository.GetAvailableDifficultyLevelsAsync();
            var difficultyLevel = difficultyLevels.FirstOrDefault(dl =>
                dl.Id == viewModel.DifficultyLevelId.Value
            );
            if (difficultyLevel == null)
            {
                throw new ArgumentException(
                    $"Difficulty level with ID {viewModel.DifficultyLevelId.Value} not found"
                );
            }

            var recipe = new Recipe
            {
                Title = viewModel.Title ?? string.Empty,
                Description = viewModel.Description ?? string.Empty,
                Instructions = viewModel.Instructions ?? string.Empty,
                PrepTimeMinutes = viewModel.PrepTimeMinutes,
                CookTimeMinutes = viewModel.CookTimeMinutes,
                Servings = viewModel.Servings,
                ImageUrl = viewModel.ImageUrl ?? string.Empty,
                DifficultyLevelId = viewModel.DifficultyLevelId.Value,
                Ingredients =
                    viewModel
                        .Ingredients?.Select(i => new Ingredient
                        {
                            Name = i.Name,
                            Amount = i.Amount,
                            Unit = i.Unit
                        })
                        .ToList() ?? new List<Ingredient>()
            };

            // For dietary tags, we'll let the repository handle the relationship
            // by passing the IDs and letting EF Core handle the many-to-many relationship
            if (viewModel.DietaryTagIds?.Any() == true)
            {
                var existingTags = await _recipeRepository.GetAvailableDietaryTagsAsync();
                recipe.DietaryTags = viewModel
                    .DietaryTagIds.Select(id => existingTags.First(t => t.Id == id))
                    .ToList();
            }

            var createdRecipe = await _recipeRepository.CreateRecipeAsync(recipe);
            return MapToRecipeDetailViewModel(createdRecipe);
        }

        public async Task<RecipeDetailViewModel> UpdateRecipeAsync(
            int id,
            UpdateRecipeViewModel viewModel
        )
        {
            if (!viewModel.DifficultyLevelId.HasValue)
            {
                throw new ArgumentException("Difficulty level ID is required");
            }

            // Validate dietary tag IDs
            var availableTags = await _recipeRepository.GetAvailableDietaryTagsAsync();
            var validTagIds = new HashSet<int>(availableTags.Select(t => t.Id));
            if (
                viewModel.DietaryTagIds != null
                && viewModel.DietaryTagIds.Any(id => !validTagIds.Contains(id))
            )
            {
                throw new ArgumentException("Invalid dietary tag ID provided");
            }

            var difficultyLevels = await _recipeRepository.GetAvailableDifficultyLevelsAsync();
            var difficultyLevel = difficultyLevels.FirstOrDefault(dl =>
                dl.Id == viewModel.DifficultyLevelId.Value
            );
            if (difficultyLevel == null)
            {
                throw new ArgumentException(
                    $"Difficulty level with ID {viewModel.DifficultyLevelId.Value} not found"
                );
            }

            var recipe = new Recipe
            {
                Id = id,
                Title = viewModel.Title ?? string.Empty,
                Description = viewModel.Description ?? string.Empty,
                Instructions = viewModel.Instructions ?? string.Empty,
                PrepTimeMinutes = viewModel.PrepTimeMinutes,
                CookTimeMinutes = viewModel.CookTimeMinutes,
                Servings = viewModel.Servings,
                ImageUrl = viewModel.ImageUrl ?? string.Empty,
                DifficultyLevelId = viewModel.DifficultyLevelId.Value,
                DietaryTags =
                    viewModel.DietaryTagIds?.Select(id => new DietaryTag { Id = id }).ToList()
                    ?? new List<DietaryTag>(),
                Ingredients =
                    viewModel
                        .Ingredients?.Select(i => new Ingredient
                        {
                            Name = i.Name,
                            Amount = i.Amount,
                            Unit = i.Unit
                        })
                        .ToList() ?? new List<Ingredient>()
            };

            var updatedRecipe = await _recipeRepository.UpdateRecipeAsync(recipe);
            if (updatedRecipe == null)
                throw new KeyNotFoundException("Recipe not found.");

            return MapToRecipeDetailViewModel(updatedRecipe);
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            return await _recipeRepository.DeleteRecipeAsync(id);
        }

        public async Task<IEnumerable<RecipeListViewModel>> GetRecipesByTotalTimeAsync(
            int maxMinutes
        )
        {
            var recipes = await _recipeRepository.GetRecipesByTotalTimeAsync(maxMinutes);
            return recipes.Select(MapToRecipeListViewModel);
        }

        public async Task<IEnumerable<RecipeListViewModel>> GetRecipesByDifficultyLevelAsync(
            string difficultyLevel
        )
        {
            var validLevels = new HashSet<string> { "Beginner", "Intermediate", "Advanced" };
            if (!validLevels.Contains(difficultyLevel))
            {
                throw new ArgumentException(
                    "Invalid difficulty level. Must be one of: Beginner, Intermediate, Advanced"
                );
            }

            var recipes = await _recipeRepository.GetRecipesByDifficultyLevelAsync(difficultyLevel);
            return recipes.Select(MapToRecipeListViewModel);
        }

        public async Task<IEnumerable<RecipeListViewModel>> GetQuickRecipesAsync(
            int maxMinutes = 30
        )
        {
            var recipes = await _recipeRepository.GetQuickRecipesAsync(maxMinutes);
            return recipes.Select(MapToRecipeListViewModel);
        }

        public async Task<IEnumerable<RecipeListViewModel>> GetRecipesByIngredientsAsync(
            IEnumerable<string> ingredients,
            bool matchAll = false
        )
        {
            if (ingredients == null || !ingredients.Any())
            {
                throw new ArgumentException("At least one ingredient must be specified");
            }

            var recipes = await _recipeRepository.GetRecipesByIngredientsAsync(
                ingredients,
                matchAll
            );
            return recipes.Select(MapToRecipeListViewModel);
        }

        public async Task<IEnumerable<DietaryTag>> GetAvailableDietaryTagsAsync()
        {
            return await _recipeRepository.GetAvailableDietaryTagsAsync();
        }

        public async Task<IEnumerable<string>> GetAvailableDifficultyLevelsAsync()
        {
            var levels = await _recipeRepository.GetAvailableDifficultyLevelsAsync();
            return levels.Select(l => l.Name);
        }

        private static RecipeListViewModel MapToRecipeListViewModel(Recipe recipe)
        {
            return new RecipeListViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                ImageUrl = recipe.ImageUrl,
                DietaryTags = recipe.DietaryTags.Select(dt => dt.Name).ToList(),
                DifficultyLevel = recipe.DifficultyLevel.Name
            };
        }

        private static RecipeDetailViewModel MapToRecipeDetailViewModel(Recipe recipe)
        {
            return new RecipeDetailViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Instructions = recipe.Instructions,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                Servings = recipe.Servings,
                ImageUrl = recipe.ImageUrl,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt,
                DietaryTags = recipe.DietaryTags.Select(dt => dt.Name).ToList(),
                DifficultyLevel = recipe.DifficultyLevel?.Name ?? string.Empty,
                Ingredients = recipe
                    .Ingredients?.Select(i => new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Amount = i.Amount,
                        Unit = i.Unit
                    })
                    .ToList()
            };
        }
    }
}

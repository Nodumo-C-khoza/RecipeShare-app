using Microsoft.AspNetCore.Mvc;
using RecipeShare.Data;
using RecipeShare.Interfaces;
using RecipeShare.Models;

namespace RecipeShare.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(IRecipeService recipeService, ILogger<RecipesController> logger)
        {
            _recipeService = recipeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<RecipeListViewModel>>> GetRecipes(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? tag = null,
            [FromQuery] string? difficulty = null,
            [FromQuery] int? maxTime = null,
            [FromQuery] bool quickRecipes = false
        )
        {
            try
            {
                if (pageNumber < 1)
                    pageNumber = 1;
                if (pageSize < 1 || pageSize > 100)
                    pageSize = 20;

                var result = await _recipeService.GetAllRecipesAsync(
                    pageNumber,
                    pageSize,
                    searchQuery,
                    tag,
                    difficulty,
                    maxTime,
                    quickRecipes
                );

                var paginatedResult = new PaginatedResult<RecipeListViewModel>
                {
                    Items = result.Recipes,
                    TotalCount = result.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
                };

                return Ok(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recipes");
                return StatusCode(500, "An error occurred while retrieving recipes");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDetailViewModel>> GetRecipe(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DietaryTag>>> GetAvailableDietaryTags()
        {
            try
            {
                var tags = await _recipeService.GetAvailableDietaryTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dietary tags");
                return StatusCode(500, "An error occurred while retrieving dietary tags");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableDifficultyLevels()
        {
            try
            {
                var levels = await _recipeService.GetAvailableDifficultyLevelsAsync();
                return Ok(levels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving difficulty levels");
                return StatusCode(500, "An error occurred while retrieving difficulty levels");
            }
        }

        [HttpPost]
        public async Task<ActionResult<RecipeDetailViewModel>> CreateRecipe(
            CreateRecipeViewModel viewModel
        )
        {
            try
            {
                var recipe = await _recipeService.CreateRecipeAsync(viewModel);
                return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RecipeDetailViewModel>> UpdateRecipe(
            int id,
            UpdateRecipeViewModel viewModel
        )
        {
            try
            {
                var recipe = await _recipeService.UpdateRecipeAsync(id, viewModel);
                if (recipe == null)
                {
                    return NotFound();
                }

                return Ok(recipe);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var result = await _recipeService.DeleteRecipeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    public class PaginatedResult<T>
    {
        public IEnumerable<T>? Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

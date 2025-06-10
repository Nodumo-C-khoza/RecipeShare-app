using Moq;
using RecipeShare.Data;
using RecipeShare.Interfaces;
using RecipeShare.Models;
using RecipeShare.Services;

namespace RecipeShareTests
{
    [TestFixture]
    public class RecipeServiceTests
    {
        private Mock<IRecipeRepository> _mockRecipeRepository;
        private RecipeService _recipeService;

        [SetUp]
        public void Setup()
        {
            _mockRecipeRepository = new Mock<IRecipeRepository>();
            _recipeService = new RecipeService(_mockRecipeRepository.Object);
        }

        [Test]
        public async Task GetAllRecipesAsync_ShouldReturnRecipesAndTotalCount()
        {
            // Arrange
            var difficultyLevel = new DifficultyLevel { Id = 1, Name = "Beginner" };
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Test Recipe 1",
                    Description = "Test Description 1",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    ImageUrl = "https://example.com/1.jpg",
                    DietaryTags = new List<DietaryTag>
                    {
                        new DietaryTag { Id = 1, Name = "Vegetarian" }
                    },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Test Recipe 2",
                    Description = "Test Description 2",
                    PrepTimeMinutes = 45,
                    CookTimeMinutes = 90,
                    ImageUrl = "https://example.com/2.jpg",
                    DietaryTags = new List<DietaryTag>
                    {
                        new DietaryTag { Id = 2, Name = "Vegan" }
                    },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                }
            };
            _mockRecipeRepository
                .Setup(repo => repo.GetAllRecipesAsync(1, 20, null, null, null, null, false))
                .ReturnsAsync((recipes, 2));

            // Act
            var (result, totalCount) = await _recipeService.GetAllRecipesAsync(
                1,
                20,
                null,
                null,
                null,
                null,
                false
            );

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(totalCount, Is.EqualTo(2));
            Assert.That(result.First().Title, Is.EqualTo("Test Recipe 1"));
            Assert.That(result.First().DifficultyLevel, Is.EqualTo("Beginner"));
            Assert.That(result.First().DietaryTags, Contains.Item("Vegetarian"));
            Assert.That(result.Last().Title, Is.EqualTo("Test Recipe 2"));
            Assert.That(result.Last().DifficultyLevel, Is.EqualTo("Beginner"));
            Assert.That(result.Last().DietaryTags, Contains.Item("Vegan"));
        }

        [Test]
        public async Task GetRecipeByIdAsync_WhenRecipeExists_ShouldReturnRecipe()
        {
            // Arrange
            var recipe = new Recipe { Id = 1, Title = "Test Recipe" };
            _mockRecipeRepository.Setup(repo => repo.GetRecipeByIdAsync(1)).ReturnsAsync(recipe);

            // Act
            var result = await _recipeService.GetRecipeByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Test Recipe"));
        }

        [Test]
        public void GetRecipeByIdAsync_WhenRecipeDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            _mockRecipeRepository
                .Setup(repo => repo.GetRecipeByIdAsync(1))
                .ReturnsAsync((Recipe)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _recipeService.GetRecipeByIdAsync(1));
        }

        [Test]
        public async Task CreateRecipeAsync_WithValidData_ShouldCreateRecipe()
        {
            // Arrange
            var createViewModel = new CreateRecipeViewModel
            {
                Title = "New Recipe",
                Description = "Test Description",
                PrepTimeMinutes = 30,
                CookTimeMinutes = 60,
                Servings = 4,
                DietaryTagIds = new List<int> { 1 }, // Using ID 1 for Vegetarian
                DifficultyLevelId = 1 // Adding required difficulty level ID
            };

            var difficultyLevel = new DifficultyLevel { Id = 1, Name = "Beginner" };
            _mockRecipeRepository
                .Setup(repo => repo.GetAvailableDifficultyLevelsAsync())
                .ReturnsAsync(new List<DifficultyLevel> { difficultyLevel });

            var dietaryTags = new List<DietaryTag>
            {
                new DietaryTag
                {
                    Id = 1,
                    Name = "Vegetarian",
                    DisplayName = "Vegetarian",
                    Description = "Vegetarian diet"
                }
            };
            _mockRecipeRepository
                .Setup(repo => repo.GetAvailableDietaryTagsAsync())
                .ReturnsAsync(dietaryTags);

            var expectedRecipe = new Recipe
            {
                Id = 1,
                Title = "New Recipe",
                Description = "Test Description",
                PrepTimeMinutes = 30,
                CookTimeMinutes = 60,
                Servings = 4,
                DietaryTags = new List<DietaryTag>
                {
                    new DietaryTag { Id = 1, Name = "Vegetarian" }
                },
                DifficultyLevelId = 1,
                DifficultyLevel = difficultyLevel
            };

            _mockRecipeRepository
                .Setup(repo => repo.CreateRecipeAsync(It.IsAny<Recipe>()))
                .ReturnsAsync(expectedRecipe);

            // Act
            var result = await _recipeService.CreateRecipeAsync(createViewModel);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("New Recipe"));
            Assert.That(result.DietaryTags, Contains.Item("Vegetarian"));
            Assert.That(result.DifficultyLevel, Is.EqualTo("Beginner"));
        }

        [Test]
        public void CreateRecipeAsync_WithInvalidCookingTime_ShouldThrowArgumentException()
        {
            // Arrange
            var createViewModel = new CreateRecipeViewModel
            {
                Title = "New Recipe",
                PrepTimeMinutes = 300,
                CookTimeMinutes = 300 // Total time > 480 minutes
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                () => _recipeService.CreateRecipeAsync(createViewModel)
            );
        }

        [Test]
        public void CreateRecipeAsync_WithInvalidDietaryTag_ShouldThrowArgumentException()
        {
            // Arrange
            var createViewModel = new CreateRecipeViewModel
            {
                Title = "New Recipe",
                DietaryTagIds = new List<int> { 999 } // Using an invalid ID
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                () => _recipeService.CreateRecipeAsync(createViewModel)
            );
        }

        [Test]
        public async Task UpdateRecipeAsync_WithValidData_ShouldUpdateRecipe()
        {
            // Arrange
            var updateViewModel = new UpdateRecipeViewModel
            {
                Title = "Updated Recipe",
                Description = "Updated Description",
                PrepTimeMinutes = 30,
                CookTimeMinutes = 60,
                Servings = 4,
                DietaryTagIds = new List<int> { 1 }, // Using ID 1 for Vegetarian
                DifficultyLevelId = 1 // Adding required difficulty level ID
            };

            var difficultyLevel = new DifficultyLevel { Id = 1, Name = "Beginner" };
            _mockRecipeRepository
                .Setup(repo => repo.GetAvailableDifficultyLevelsAsync())
                .ReturnsAsync(new List<DifficultyLevel> { difficultyLevel });

            var expectedRecipe = new Recipe
            {
                Id = 1,
                Title = "Updated Recipe",
                Description = "Updated Description",
                PrepTimeMinutes = 30,
                CookTimeMinutes = 60,
                Servings = 4,
                DietaryTags = new List<DietaryTag>
                {
                    new DietaryTag { Id = 1, Name = "Vegetarian" }
                },
                DifficultyLevelId = 1,
                DifficultyLevel = difficultyLevel
            };

            _mockRecipeRepository
                .Setup(repo => repo.UpdateRecipeAsync(It.IsAny<Recipe>()))
                .ReturnsAsync(expectedRecipe);

            // Act
            var result = await _recipeService.UpdateRecipeAsync(1, updateViewModel);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Updated Recipe"));
            Assert.That(result.DietaryTags, Contains.Item("Vegetarian"));
            Assert.That(result.DifficultyLevel, Is.EqualTo("Beginner"));
        }

        [Test]
        public async Task DeleteRecipeAsync_WhenRecipeExists_ShouldReturnTrue()
        {
            // Arrange
            _mockRecipeRepository.Setup(repo => repo.DeleteRecipeAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _recipeService.DeleteRecipeAsync(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetRecipesByDietaryTagAsync_ShouldReturnFilteredRecipes()
        {
            // Arrange
            var difficultyLevel = new DifficultyLevel { Id = 1, Name = "Beginner" };
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Veg Recipe",
                    Description = "Vegetarian Recipe",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    ImageUrl = "https://example.com/veg.jpg",
                    DietaryTags = new List<DietaryTag>
                    {
                        new DietaryTag { Id = 1, Name = "Vegetarian" }
                    },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Vegan Recipe",
                    Description = "Vegan Recipe",
                    PrepTimeMinutes = 45,
                    CookTimeMinutes = 90,
                    ImageUrl = "https://example.com/vegan.jpg",
                    DietaryTags = new List<DietaryTag>
                    {
                        new DietaryTag { Id = 2, Name = "Vegan" }
                    },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                }
            };
            _mockRecipeRepository
                .Setup(repo => repo.GetRecipesByDietaryTagAsync("Vegetarian"))
                .ReturnsAsync(recipes.Where(r => r.DietaryTags.Any(dt => dt.Name == "Vegetarian")));

            // Act
            var result = await _recipeService.GetRecipesByDietaryTagAsync("Vegetarian");

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Veg Recipe"));
            Assert.That(result.First().DifficultyLevel, Is.EqualTo("Beginner"));
            Assert.That(result.First().DietaryTags, Contains.Item("Vegetarian"));
        }

        [Test]
        public void GetRecipesByDifficultyLevelAsync_WithInvalidLevel_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                () => _recipeService.GetRecipesByDifficultyLevelAsync("InvalidLevel")
            );
        }

        [Test]
        public void GetRecipesByIngredientsAsync_WithEmptyIngredients_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                () => _recipeService.GetRecipesByIngredientsAsync(new List<string>())
            );
        }
    }
}

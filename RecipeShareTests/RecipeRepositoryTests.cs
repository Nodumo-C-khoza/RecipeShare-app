using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeShare.Data;
using RecipeShare.Repository;

namespace RecipeShareTests
{
    [TestFixture]
    public class RecipeRepositoryTests
    {
        private ApplicationDbContext _context;
        private RecipeRepository _repository;
        private Mock<IEasyCachingProvider> _mockCache;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockCache = new Mock<IEasyCachingProvider>();
            _repository = new RecipeRepository(_context, _mockCache.Object);

            // Setup cache mock to return empty values by default
            _mockCache
                .Setup(x => x.GetAsync<Recipe>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CacheValue<Recipe>(null, false));
            _mockCache
                .Setup(x =>
                    x.GetAsync<(IEnumerable<Recipe>, int)>(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(new CacheValue<(IEnumerable<Recipe>, int)>((null, 0), false));

            // Seed test data
            _context.Recipes.AddRange(
                new Recipe
                {
                    Id = 1,
                    Title = "Test Recipe 1",
                    Description = "Description 1",
                    Instructions = "Instructions for recipe 1",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    Servings = 4,
                    ImageUrl = "https://example.com/recipe1.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag>
                    {
                        new DietaryTag
                        {
                            Id = 1,
                            Name = "Vegetarian",
                            DisplayName = "Vegetarian",
                            Description = "Vegetarian diet"
                        }
                    },
                    DifficultyLevelId = 1,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Ingredient 1",
                            Amount = "1",
                            Unit = "cup"
                        },
                        new Ingredient
                        {
                            Name = "Ingredient 2",
                            Amount = "2",
                            Unit = "tbsp"
                        }
                    }
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Test Recipe 2",
                    Description = "Description 2",
                    Instructions = "Instructions for recipe 2",
                    PrepTimeMinutes = 45,
                    CookTimeMinutes = 90,
                    Servings = 6,
                    ImageUrl = "https://example.com/recipe2.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag>
                    {
                        new DietaryTag
                        {
                            Id = 2,
                            Name = "Vegan",
                            DisplayName = "Vegan",
                            Description = "Vegan diet"
                        }
                    },
                    DifficultyLevelId = 2,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Ingredient 3",
                            Amount = "3",
                            Unit = "oz"
                        },
                        new Ingredient
                        {
                            Name = "Ingredient 4",
                            Amount = "4",
                            Unit = "g"
                        }
                    }
                }
            );
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllRecipesAsync_ShouldReturnAllRecipes()
        {
            // Arrange
            // Clear existing data
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            _context.DifficultyLevels.RemoveRange(_context.DifficultyLevels);
            await _context.SaveChangesAsync();

            // Add difficulty levels first
            var difficultyLevels = new List<DifficultyLevel>
            {
                new DifficultyLevel
                {
                    Id = 1,
                    Name = "Beginner",
                    Description = "Easy recipes"
                },
                new DifficultyLevel
                {
                    Id = 2,
                    Name = "Intermediate",
                    Description = "Moderate recipes"
                }
            };
            _context.DifficultyLevels.AddRange(difficultyLevels);
            await _context.SaveChangesAsync();

            // Add dietary tags
            var dietaryTags = new List<DietaryTag>
            {
                new DietaryTag
                {
                    Id = 1,
                    Name = "Vegetarian",
                    DisplayName = "Vegetarian",
                    Description = "Vegetarian diet"
                },
                new DietaryTag
                {
                    Id = 2,
                    Name = "Vegan",
                    DisplayName = "Vegan",
                    Description = "Vegan diet"
                },
                new DietaryTag
                {
                    Id = 3,
                    Name = "Gluten-Free",
                    DisplayName = "Gluten-Free",
                    Description = "Gluten-free diet"
                }
            };
            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Test Recipe 1",
                    Description = "Description 1",
                    Instructions = "Instructions for recipe 1",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    Servings = 4,
                    ImageUrl = "https://example.com/recipe1.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevels[0],
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Ingredient 1",
                            Amount = "1",
                            Unit = "cup"
                        }
                    }
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Test Recipe 2",
                    Description = "Description 2",
                    Instructions = "Instructions for recipe 2",
                    PrepTimeMinutes = 45,
                    CookTimeMinutes = 90,
                    Servings = 6,
                    ImageUrl = "https://example.com/recipe2.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[1] },
                    DifficultyLevelId = 2,
                    DifficultyLevel = difficultyLevels[1],
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Ingredient 2",
                            Amount = "2",
                            Unit = "tbsp"
                        }
                    }
                },
                new Recipe
                {
                    Id = 3,
                    Title = "Test Recipe 3",
                    Description = "Description 3",
                    Instructions = "Instructions for recipe 3",
                    PrepTimeMinutes = 20,
                    CookTimeMinutes = 40,
                    Servings = 2,
                    ImageUrl = "https://example.com/recipe3.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[2] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevels[0],
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Ingredient 3",
                            Amount = "3",
                            Unit = "oz"
                        }
                    }
                }
            };

            _context.Recipes.AddRange(recipes);
            await _context.SaveChangesAsync();

            // Act
            var (result, totalCount) = await _repository.GetAllRecipesAsync(1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(3));
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(r => r.Title == "Test Recipe 1"), Is.True);
            Assert.That(result.Any(r => r.Title == "Test Recipe 2"), Is.True);
            Assert.That(result.Any(r => r.Title == "Test Recipe 3"), Is.True);
        }

        [Test]
        public async Task GetRecipeByIdAsync_WhenRecipeExists_ShouldReturnRecipe()
        {
            // Arrange
            // Clear existing data
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            _context.DifficultyLevels.RemoveRange(_context.DifficultyLevels);
            await _context.SaveChangesAsync();

            // Add difficulty level
            var difficultyLevel = new DifficultyLevel
            {
                Id = 1,
                Name = "Beginner",
                Description = "Easy recipes"
            };
            _context.DifficultyLevels.Add(difficultyLevel);
            await _context.SaveChangesAsync();

            // Add dietary tags
            var dietaryTags = new List<DietaryTag>
            {
                new DietaryTag
                {
                    Id = 1,
                    Name = "Vegetarian",
                    DisplayName = "Vegetarian",
                    Description = "Vegetarian diet"
                },
                new DietaryTag
                {
                    Id = 2,
                    Name = "Vegan",
                    DisplayName = "Vegan",
                    Description = "Vegan diet"
                }
            };
            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            // Create test recipe
            var recipe = new Recipe
            {
                Id = 1,
                Title = "Test Recipe",
                Description = "Test Description",
                Instructions = "Test Instructions",
                PrepTimeMinutes = 30,
                CookTimeMinutes = 60,
                Servings = 4,
                ImageUrl = "https://example.com/test.jpg",
                CreatedAt = DateTime.UtcNow,
                DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                DifficultyLevelId = 1,
                DifficultyLevel = difficultyLevel,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Test Ingredient",
                        Amount = "1",
                        Unit = "cup"
                    }
                }
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRecipeByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Test Recipe"));
            Assert.That(result.Description, Is.EqualTo("Test Description"));
            Assert.That(result.Instructions, Is.EqualTo("Test Instructions"));
            Assert.That(result.PrepTimeMinutes, Is.EqualTo(30));
            Assert.That(result.CookTimeMinutes, Is.EqualTo(60));
            Assert.That(result.Servings, Is.EqualTo(4));
            Assert.That(result.ImageUrl, Is.EqualTo("https://example.com/test.jpg"));
            Assert.That(result.DifficultyLevelId, Is.EqualTo(1));
            Assert.That(result.DifficultyLevel, Is.Not.Null);
            Assert.That(result.DifficultyLevel.Name, Is.EqualTo("Beginner"));
            Assert.That(result.DietaryTags, Is.Not.Null);
            Assert.That(result.DietaryTags.Count, Is.EqualTo(1));
            Assert.That(result.DietaryTags.First().Name, Is.EqualTo("Vegetarian"));
            Assert.That(result.Ingredients, Is.Not.Null);
            Assert.That(result.Ingredients.Count, Is.EqualTo(1));
            Assert.That(result.Ingredients.First().Name, Is.EqualTo("Test Ingredient"));
        }

        [Test]
        public async Task GetRecipeByIdAsync_WhenRecipeDoesNotExist_ShouldReturnNull()
        {
            // Act
            var recipe = await _repository.GetRecipeByIdAsync(999);

            // Assert
            Assert.That(recipe, Is.Null);
        }

        [Test]
        public async Task CreateRecipeAsync_ShouldAddNewRecipe()
        {
            // Arrange
            var newRecipe = new Recipe
            {
                Title = "New Recipe",
                Description = "New Description",
                Instructions = "New Instructions",
                PrepTimeMinutes = 20,
                CookTimeMinutes = 40,
                Servings = 2,
                ImageUrl = "https://example.com/new-recipe.jpg",
                CreatedAt = DateTime.UtcNow,
                DietaryTags = new List<DietaryTag>
                {
                    new DietaryTag
                    {
                        Id = 4,
                        Name = "Gluten-Free",
                        DisplayName = "Gluten-Free",
                        Description = "Gluten-free diet"
                    }
                },
                DifficultyLevelId = 1,
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "New Ingredient",
                        Amount = "1",
                        Unit = "tsp"
                    }
                }
            };

            // Act
            var createdRecipe = await _repository.CreateRecipeAsync(newRecipe);

            // Assert
            Assert.That(createdRecipe.Id, Is.GreaterThan(0));
            Assert.That(createdRecipe.Title, Is.EqualTo("New Recipe"));
            Assert.That(_context.Recipes.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task UpdateRecipeAsync_WhenRecipeExists_ShouldUpdateRecipe()
        {
            // Arrange
            // Clear existing data
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            _context.DifficultyLevels.RemoveRange(_context.DifficultyLevels);
            await _context.SaveChangesAsync();

            // Add difficulty levels
            var difficultyLevels = new List<DifficultyLevel>
            {
                new DifficultyLevel
                {
                    Id = 1,
                    Name = "Beginner",
                    Description = "Easy recipes"
                },
                new DifficultyLevel
                {
                    Id = 2,
                    Name = "Intermediate",
                    Description = "Moderate recipes"
                }
            };
            _context.DifficultyLevels.AddRange(difficultyLevels);
            await _context.SaveChangesAsync();

            // Add dietary tags
            var dietaryTags = new List<DietaryTag>
            {
                new DietaryTag
                {
                    Id = 1,
                    Name = "Vegetarian",
                    DisplayName = "Vegetarian",
                    Description = "Vegetarian diet"
                },
                new DietaryTag
                {
                    Id = 2,
                    Name = "Vegan",
                    DisplayName = "Vegan",
                    Description = "Vegan diet"
                }
            };
            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            // Create initial recipe
            var initialRecipe = new Recipe
            {
                Id = 1,
                Title = "Original Recipe",
                Description = "Original Description",
                Instructions = "Original Instructions",
                PrepTimeMinutes = 30,
                CookTimeMinutes = 60,
                Servings = 4,
                ImageUrl = "https://example.com/original.jpg",
                CreatedAt = DateTime.UtcNow,
                DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                DifficultyLevelId = 1,
                DifficultyLevel = difficultyLevels[0],
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Original Ingredient",
                        Amount = "1",
                        Unit = "cup"
                    }
                }
            };

            _context.Recipes.Add(initialRecipe);
            await _context.SaveChangesAsync();

            // Create updated recipe
            var updatedRecipe = new Recipe
            {
                Id = 1,
                Title = "Updated Recipe",
                Description = "Updated Description",
                Instructions = "Updated Instructions",
                PrepTimeMinutes = 45,
                CookTimeMinutes = 90,
                Servings = 6,
                ImageUrl = "https://example.com/updated.jpg",
                CreatedAt = initialRecipe.CreatedAt,
                DietaryTags = new List<DietaryTag> { dietaryTags[1] },
                DifficultyLevelId = 2,
                DifficultyLevel = difficultyLevels[1],
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Updated Ingredient",
                        Amount = "2",
                        Unit = "tbsp"
                    }
                }
            };

            // Act
            var result = await _repository.UpdateRecipeAsync(updatedRecipe);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Updated Recipe"));
            Assert.That(result.Description, Is.EqualTo("Updated Description"));
            Assert.That(result.Instructions, Is.EqualTo("Updated Instructions"));
            Assert.That(result.PrepTimeMinutes, Is.EqualTo(45));
            Assert.That(result.CookTimeMinutes, Is.EqualTo(90));
            Assert.That(result.Servings, Is.EqualTo(6));
            Assert.That(result.ImageUrl, Is.EqualTo("https://example.com/updated.jpg"));
            Assert.That(result.DifficultyLevelId, Is.EqualTo(2));
            Assert.That(result.DietaryTags.Count, Is.EqualTo(1));
            Assert.That(result.DietaryTags.First().Name, Is.EqualTo("Vegan"));
            Assert.That(result.Ingredients.Count, Is.EqualTo(1));
            Assert.That(result.Ingredients.First().Name, Is.EqualTo("Updated Ingredient"));
        }

        [Test]
        public async Task DeleteRecipeAsync_WhenRecipeExists_ShouldRemoveRecipe()
        {
            // Act
            var result = await _repository.DeleteRecipeAsync(1);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_context.Recipes.Count(), Is.EqualTo(1));
            Assert.That(await _repository.GetRecipeByIdAsync(1), Is.Null);
        }

        [Test]
        public async Task GetRecipesByDietaryTagAsync_ShouldReturnFilteredRecipes()
        {
            // Arrange
            // Clear existing data
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            _context.DifficultyLevels.RemoveRange(_context.DifficultyLevels);
            await _context.SaveChangesAsync();

            // Add difficulty level
            var difficultyLevel = new DifficultyLevel
            {
                Id = 1,
                Name = "Beginner",
                Description = "Easy recipes"
            };
            _context.DifficultyLevels.Add(difficultyLevel);
            await _context.SaveChangesAsync();

            // Add dietary tags
            var dietaryTags = new List<DietaryTag>
            {
                new DietaryTag
                {
                    Id = 1,
                    Name = "Vegetarian",
                    DisplayName = "Vegetarian",
                    Description = "Vegetarian diet"
                },
                new DietaryTag
                {
                    Id = 2,
                    Name = "Vegan",
                    DisplayName = "Vegan",
                    Description = "Vegan diet"
                }
            };
            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            // Add recipes
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Veg Recipe",
                    Description = "A vegetarian recipe",
                    Instructions = "Vegetarian instructions",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    Servings = 4,
                    ImageUrl = "https://example.com/veg.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] }, // Vegetarian
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Vegan Recipe",
                    Description = "A vegan recipe",
                    Instructions = "Vegan instructions",
                    PrepTimeMinutes = 45,
                    CookTimeMinutes = 90,
                    Servings = 6,
                    ImageUrl = "https://example.com/vegan.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[1] }, // Vegan
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                }
            };

            _context.Recipes.AddRange(recipes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRecipesByDietaryTagAsync("Vegetarian");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Veg Recipe"));
            Assert.That(result.First().DietaryTags.First().Name, Is.EqualTo("Vegetarian"));
        }

        [Test]
        public async Task GetRecipesByTotalTimeAsync_ShouldReturnFilteredRecipes()
        {
            // Arrange
            // Clear existing data
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            _context.DifficultyLevels.RemoveRange(_context.DifficultyLevels);
            await _context.SaveChangesAsync();

            // Add difficulty level
            var difficultyLevel = new DifficultyLevel
            {
                Id = 1,
                Name = "Beginner",
                Description = "Easy recipes"
            };
            _context.DifficultyLevels.Add(difficultyLevel);
            await _context.SaveChangesAsync();

            // Add dietary tags
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
            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            // Add recipes
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Quick Recipe",
                    Description = "A quick recipe",
                    Instructions = "Quick instructions",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 15,
                    Servings = 2,
                    ImageUrl = "https://example.com/quick.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Long Recipe",
                    Description = "A long recipe",
                    Instructions = "Long instructions",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    Servings = 4,
                    ImageUrl = "https://example.com/long.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                }
            };

            _context.Recipes.AddRange(recipes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRecipesByTotalTimeAsync(30); // Total time <= 30 minutes

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Quick Recipe"));
            Assert.That(
                result.First().PrepTimeMinutes + result.First().CookTimeMinutes,
                Is.LessThanOrEqualTo(30)
            );
        }

        [Test]
        public async Task GetRecipesByDifficultyLevelAsync_ShouldReturnFilteredRecipes()
        {
            // Arrange
            // Clear existing data
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            _context.DifficultyLevels.RemoveRange(_context.DifficultyLevels);
            await _context.SaveChangesAsync();

            // Add difficulty levels
            var difficultyLevels = new List<DifficultyLevel>
            {
                new DifficultyLevel
                {
                    Id = 1,
                    Name = "Beginner",
                    Description = "Easy recipes"
                },
                new DifficultyLevel
                {
                    Id = 2,
                    Name = "Intermediate",
                    Description = "Moderate recipes"
                }
            };
            _context.DifficultyLevels.AddRange(difficultyLevels);
            await _context.SaveChangesAsync();

            // Add dietary tags
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
            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            // Add recipes
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Beginner Recipe",
                    Description = "An easy recipe",
                    Instructions = "Beginner instructions",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    Servings = 4,
                    ImageUrl = "https://example.com/beginner.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevels[0] // Beginner
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Intermediate Recipe",
                    Description = "A moderate recipe",
                    Instructions = "Intermediate instructions",
                    PrepTimeMinutes = 45,
                    CookTimeMinutes = 90,
                    Servings = 6,
                    ImageUrl = "https://example.com/intermediate.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 2,
                    DifficultyLevel = difficultyLevels[1] // Intermediate
                }
            };

            _context.Recipes.AddRange(recipes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRecipesByDifficultyLevelAsync("Beginner");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Beginner Recipe"));
            Assert.That(result.First().DifficultyLevel.Name, Is.EqualTo("Beginner"));
        }

        [Test]
        public async Task GetQuickRecipesAsync_ShouldReturnFilteredRecipes()
        {
            // Arrange
            // Clear existing data
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            _context.DifficultyLevels.RemoveRange(_context.DifficultyLevels);
            await _context.SaveChangesAsync();

            // Add difficulty level
            var difficultyLevel = new DifficultyLevel
            {
                Id = 1,
                Name = "Beginner",
                Description = "Easy recipes"
            };
            _context.DifficultyLevels.Add(difficultyLevel);
            await _context.SaveChangesAsync();

            // Add dietary tags
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
            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            // Add recipes
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Quick Recipe",
                    Description = "A quick recipe",
                    Instructions = "Quick instructions",
                    PrepTimeMinutes = 10,
                    CookTimeMinutes = 15,
                    Servings = 2,
                    ImageUrl = "https://example.com/quick.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Long Recipe",
                    Description = "A long recipe",
                    Instructions = "Long instructions",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    Servings = 4,
                    ImageUrl = "https://example.com/long.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel
                }
            };

            _context.Recipes.AddRange(recipes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetQuickRecipesAsync(30);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Quick Recipe"));
            Assert.That(
                result.First().PrepTimeMinutes + result.First().CookTimeMinutes,
                Is.LessThanOrEqualTo(30)
            );
        }

        [Test]
        public async Task GetRecipesByIngredientsAsync_WithMatchAll_ShouldReturnMatchingRecipes()
        {
            // Arrange
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            _context.DifficultyLevels.RemoveRange(_context.DifficultyLevels);
            await _context.SaveChangesAsync();

            var difficultyLevel = new DifficultyLevel
            {
                Id = 1,
                Name = "Beginner",
                Description = "Easy recipes"
            };
            _context.DifficultyLevels.Add(difficultyLevel);
            await _context.SaveChangesAsync();

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
            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            // Add recipes with ingredients
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Title = "Pasta Recipe",
                    Description = "A pasta recipe",
                    Instructions = "Pasta instructions",
                    PrepTimeMinutes = 30,
                    CookTimeMinutes = 60,
                    Servings = 4,
                    ImageUrl = "https://example.com/pasta.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Pasta",
                            Amount = "500",
                            Unit = "g"
                        },
                        new Ingredient
                        {
                            Name = "Tomato Sauce",
                            Amount = "400",
                            Unit = "g"
                        },
                        new Ingredient
                        {
                            Name = "Cheese",
                            Amount = "200",
                            Unit = "g"
                        }
                    }
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Salad Recipe",
                    Description = "A salad recipe",
                    Instructions = "Salad instructions",
                    PrepTimeMinutes = 15,
                    CookTimeMinutes = 0,
                    Servings = 2,
                    ImageUrl = "https://example.com/salad.jpg",
                    CreatedAt = DateTime.UtcNow,
                    DietaryTags = new List<DietaryTag> { dietaryTags[0] },
                    DifficultyLevelId = 1,
                    DifficultyLevel = difficultyLevel,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Lettuce",
                            Amount = "1",
                            Unit = "head"
                        },
                        new Ingredient
                        {
                            Name = "Tomato",
                            Amount = "2",
                            Unit = "pieces"
                        },
                        new Ingredient
                        {
                            Name = "Cucumber",
                            Amount = "1",
                            Unit = "piece"
                        }
                    }
                }
            };

            _context.Recipes.AddRange(recipes);
            await _context.SaveChangesAsync();

            // Act
            var searchIngredients = new List<string> { "Pasta", "Tomato Sauce" };
            var result = await _repository.GetRecipesByIngredientsAsync(searchIngredients, true);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Pasta Recipe"));
            Assert.That(result.First().Ingredients.Count, Is.EqualTo(3));
            Assert.That(result.First().Ingredients.Any(i => i.Name == "Pasta"), Is.True);
            Assert.That(result.First().Ingredients.Any(i => i.Name == "Tomato Sauce"), Is.True);
        }

        [Test]
        public async Task GetAvailableDietaryTagsAsync_ShouldReturnAllTags()
        {
            // Arrange
            _context.DietaryTags.RemoveRange(_context.DietaryTags);
            await _context.SaveChangesAsync();

            var dietaryTags = new List<DietaryTag>
            {
                new DietaryTag
                {
                    Id = 1,
                    Name = "Vegetarian",
                    DisplayName = "Vegetarian",
                    Description = "Vegetarian diet"
                },
                new DietaryTag
                {
                    Id = 2,
                    Name = "Vegan",
                    DisplayName = "Vegan",
                    Description = "Vegan diet"
                },
                new DietaryTag
                {
                    Id = 3,
                    Name = "Gluten-Free",
                    DisplayName = "Gluten-Free",
                    Description = "Gluten-free diet"
                }
            };

            _context.DietaryTags.AddRange(dietaryTags);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAvailableDietaryTagsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Select(t => t.Name), Contains.Item("Vegetarian"));
            Assert.That(result.Select(t => t.Name), Contains.Item("Vegan"));
            Assert.That(result.Select(t => t.Name), Contains.Item("Gluten-Free"));
        }

        [Test]
        public async Task GetAvailableDifficultyLevelsAsync_ShouldReturnAllLevels()
        {
            // Arrange
            var levels = new List<DifficultyLevel>
            {
                new DifficultyLevel
                {
                    Id = 1,
                    Name = "Beginner",
                    Description = "Easy recipes"
                },
                new DifficultyLevel
                {
                    Id = 2,
                    Name = "Intermediate",
                    Description = "Moderate recipes"
                },
                new DifficultyLevel
                {
                    Id = 3,
                    Name = "Advanced",
                    Description = "Complex recipes"
                }
            };
            _context.DifficultyLevels.AddRange(levels);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAvailableDifficultyLevelsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(l => l.Name == "Beginner"), Is.True);
            Assert.That(result.Any(l => l.Name == "Intermediate"), Is.True);
            Assert.That(result.Any(l => l.Name == "Advanced"), Is.True);
        }
    }
}

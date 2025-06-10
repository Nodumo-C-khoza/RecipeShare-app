using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace RecipeShare.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<DifficultyLevel> DifficultyLevels { get; set; }
        public DbSet<DietaryTag> DietaryTags { get; set; }
        public DbSet<RecipeDietaryTag> RecipeDietaryTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add indexes for frequently queried fields
            modelBuilder.Entity<Recipe>().HasIndex(r => r.Title);

            modelBuilder.Entity<Recipe>().HasIndex(r => r.CreatedAt);

            modelBuilder
                .Entity<Recipe>()
                .HasIndex(r => new { r.PrepTimeMinutes, r.CookTimeMinutes });

            modelBuilder.Entity<DietaryTag>().HasIndex(dt => dt.Name);

            modelBuilder.Entity<DifficultyLevel>().HasIndex(dl => dl.Name);

            // Configure Recipe entity
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(r => r.Id);

                // Configure required fields
                entity.Property(r => r.Title).IsRequired();
                entity.Property(r => r.Description).IsRequired();
                entity.Property(r => r.Instructions).IsRequired();
                entity.Property(r => r.ImageUrl).IsRequired();

                // Configure Recipe-DifficultyLevel relationship
                entity
                    .HasOne(r => r.DifficultyLevel)
                    .WithMany()
                    .HasForeignKey(r => r.DifficultyLevelId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure Recipe-DietaryTag many-to-many relationship
                entity
                    .HasMany(r => r.DietaryTags)
                    .WithMany(d => d.Recipes)
                    .UsingEntity<RecipeDietaryTag>(
                        j =>
                            j.HasOne(rdt => rdt.DietaryTag)
                                .WithMany()
                                .HasForeignKey(rdt => rdt.DietaryTagId),
                        j =>
                            j.HasOne(rdt => rdt.Recipe)
                                .WithMany()
                                .HasForeignKey(rdt => rdt.RecipeId)
                    );

                // Configure Recipe-Ingredient relationship
                entity
                    .HasMany(r => r.Ingredients)
                    .WithOne(i => i.Recipe)
                    .HasForeignKey(i => i.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Ingredient entity
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Name).IsRequired();
                entity.HasIndex(i => i.Name);
                entity.HasIndex(i => i.RecipeId);
            });

            // Configure DifficultyLevel entity
            modelBuilder.Entity<DifficultyLevel>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired();
                entity.Property(d => d.DisplayName).IsRequired();
                entity.HasIndex(d => d.Name);
            });

            // Configure DietaryTag entity
            modelBuilder.Entity<DietaryTag>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired();
                entity.Property(d => d.DisplayName).IsRequired();
                entity.HasIndex(d => d.Name);
            });

            // Configure RecipeDietaryTag entity
            modelBuilder.Entity<RecipeDietaryTag>(entity =>
            {
                entity.HasKey(rdt => new { rdt.RecipeId, rdt.DietaryTagId });
            });

            // Seed Difficulty Levels
            modelBuilder
                .Entity<DifficultyLevel>()
                .HasData(
                    new DifficultyLevel
                    {
                        Id = 1,
                        Name = "Beginner",
                        DisplayName = "Beginner (Easy)",
                        Description = "Simple recipes with basic techniques and few ingredients"
                    },
                    new DifficultyLevel
                    {
                        Id = 2,
                        Name = "Intermediate",
                        DisplayName = "Intermediate (Medium)",
                        Description =
                            "Recipes requiring some cooking experience and moderate techniques"
                    },
                    new DifficultyLevel
                    {
                        Id = 3,
                        Name = "Advanced",
                        DisplayName = "Advanced (Hard)",
                        Description = "Complex recipes requiring advanced techniques and experience"
                    }
                );

            // Seed Dietary Tags
            modelBuilder
                .Entity<DietaryTag>()
                .HasData(
                    new DietaryTag
                    {
                        Id = 1,
                        Name = "Vegetarian",
                        DisplayName = "Vegetarian",
                        Description = "Contains no meat or fish"
                    },
                    new DietaryTag
                    {
                        Id = 2,
                        Name = "Vegan",
                        DisplayName = "Vegan",
                        Description = "Contains no animal products"
                    },
                    new DietaryTag
                    {
                        Id = 3,
                        Name = "GlutenFree",
                        DisplayName = "Gluten-Free",
                        Description = "Contains no gluten"
                    },
                    new DietaryTag
                    {
                        Id = 4,
                        Name = "DairyFree",
                        DisplayName = "Dairy-Free",
                        Description = "Contains no dairy products"
                    },
                    new DietaryTag
                    {
                        Id = 5,
                        Name = "HighProtein",
                        DisplayName = "High Protein",
                        Description = "High in protein content"
                    },
                    new DietaryTag
                    {
                        Id = 6,
                        Name = "LowCarb",
                        DisplayName = "Low-Carb",
                        Description = "Low in carbohydrates"
                    },
                    new DietaryTag
                    {
                        Id = 7,
                        Name = "NutFree",
                        DisplayName = "Nut-Free",
                        Description = "Contains no nuts"
                    }
                );

            // Seed data with real images from TheMealDB
            modelBuilder
                .Entity<Recipe>()
                .HasData(
                    new Recipe
                    {
                        Id = 1,
                        Title = "Classic Margherita Pizza",
                        Description = "A simple and delicious traditional Italian pizza",
                        Instructions =
                            "1. Preheat oven to 450°F\n2. Roll out pizza dough\n3. Add sauce and toppings\n4. Bake for 12-15 minutes",
                        PrepTimeMinutes = 20,
                        CookTimeMinutes = 15,
                        Servings = 4,
                        ImageUrl =
                            "https://www.themealdb.com/images/media/meals/x0lk931587671540.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 1
                    },
                    new Recipe
                    {
                        Id = 2,
                        Title = "Chicken Stir Fry",
                        Description = "Quick and healthy Asian-inspired dish",
                        Instructions =
                            "1. Cut chicken into strips\n2. Stir fry vegetables\n3. Add chicken and sauce\n4. Serve over rice",
                        PrepTimeMinutes = 15,
                        CookTimeMinutes = 20,
                        Servings = 4,
                        ImageUrl = "https://www.themealdb.com/images/media/meals/1520084413.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 2
                    },
                    new Recipe
                    {
                        Id = 3,
                        Title = "Chocolate Chip Cookies",
                        Description = "Classic homemade cookies",
                        Instructions =
                            "1. Mix dry ingredients\n2. Cream butter and sugar\n3. Add chocolate chips\n4. Bake at 350°F for 10-12 minutes",
                        PrepTimeMinutes = 15,
                        CookTimeMinutes = 12,
                        Servings = 24,
                        ImageUrl =
                            "https://www.themealdb.com/images/media/meals/wyrqqq1468233628.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 1
                    },
                    new Recipe
                    {
                        Id = 4,
                        Title = "Vegetarian Buddha Bowl",
                        Description = "A nourishing bowl packed with vegetables and protein",
                        Instructions =
                            "1. Cook quinoa according to package instructions\n2. Roast vegetables with olive oil and seasonings\n3. Prepare tahini dressing\n4. Assemble bowl with all components",
                        PrepTimeMinutes = 20,
                        CookTimeMinutes = 30,
                        Servings = 2,
                        ImageUrl = "https://www.themealdb.com/images/media/meals/1550441882.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 1
                    },
                    new Recipe
                    {
                        Id = 5,
                        Title = "Beef Wellington",
                        Description =
                            "Classic British dish with beef fillet wrapped in puff pastry",
                        Instructions =
                            "1. Sear beef fillet\n2. Prepare mushroom duxelles\n3. Wrap in prosciutto and puff pastry\n4. Bake until golden brown",
                        PrepTimeMinutes = 45,
                        CookTimeMinutes = 40,
                        Servings = 4,
                        ImageUrl =
                            "https://www.themealdb.com/images/media/meals/vvusxs1483907034.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 3
                    },
                    new Recipe
                    {
                        Id = 6,
                        Title = "Vegan Pad Thai",
                        Description = "Plant-based version of the classic Thai noodle dish",
                        Instructions =
                            "1. Soak rice noodles\n2. Prepare tofu and vegetables\n3. Make pad thai sauce\n4. Stir fry all components together",
                        PrepTimeMinutes = 25,
                        CookTimeMinutes = 15,
                        Servings = 4,
                        ImageUrl =
                            "https://www.themealdb.com/images/media/meals/vtxyxv1483567157.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 2
                    },
                    new Recipe
                    {
                        Id = 11,
                        Title = "Gluten-Free Chocolate Cake",
                        Description = "Rich and moist chocolate cake without gluten",
                        Instructions =
                            "1. Mix dry ingredients\n2. Combine wet ingredients\n3. Bake at 350°F\n4. Prepare chocolate ganache",
                        PrepTimeMinutes = 20,
                        CookTimeMinutes = 35,
                        Servings = 12,
                        ImageUrl = "https://www.themealdb.com/images/media/meals/1550441275.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 2
                    },
                    new Recipe
                    {
                        Id = 8,
                        Title = "Mediterranean Grilled Salmon",
                        Description = "Healthy salmon with Mediterranean flavors",
                        Instructions =
                            "1. Marinate salmon\n2. Prepare vegetable medley\n3. Grill salmon\n4. Serve with vegetables",
                        PrepTimeMinutes = 15,
                        CookTimeMinutes = 20,
                        Servings = 4,
                        ImageUrl = "https://www.themealdb.com/images/media/meals/1550441275.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 2
                    },
                    new Recipe
                    {
                        Id = 9,
                        Title = "Quinoa Buddha Bowl",
                        Description = "Protein-packed bowl with quinoa and vegetables",
                        Instructions =
                            "1. Cook quinoa\n2. Roast vegetables\n3. Prepare tahini dressing\n4. Assemble bowl",
                        PrepTimeMinutes = 20,
                        CookTimeMinutes = 25,
                        Servings = 2,
                        ImageUrl = "https://www.themealdb.com/images/media/meals/1550441882.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 1
                    },
                    new Recipe
                    {
                        Id = 10,
                        Title = "Low-Carb Cauliflower Rice",
                        Description = "Healthy alternative to traditional rice",
                        Instructions =
                            "1. Process cauliflower\n2. Cook with seasonings\n3. Add vegetables\n4. Serve hot",
                        PrepTimeMinutes = 10,
                        CookTimeMinutes = 15,
                        Servings = 4,
                        ImageUrl = "https://www.themealdb.com/images/media/meals/1550441882.jpg",
                        CreatedAt = new DateTime(2025, 1, 1),
                        DifficultyLevelId = 1
                    }
                );

            // Seed Recipe-DietaryTag relationships
            modelBuilder
                .Entity<RecipeDietaryTag>()
                .HasData(
                    // Margherita Pizza tags
                    new RecipeDietaryTag { RecipeId = 1, DietaryTagId = 1 }, // Vegetarian
                    new RecipeDietaryTag { RecipeId = 1, DietaryTagId = 4 }, // Dairy-Free
                    // Chicken Stir Fry tags
                    new RecipeDietaryTag { RecipeId = 2, DietaryTagId = 5 }, // High Protein
                    // Chocolate Chip Cookies tags
                    new RecipeDietaryTag { RecipeId = 3, DietaryTagId = 1 }, // Vegetarian
                    // Buddha Bowl tags
                    new RecipeDietaryTag { RecipeId = 4, DietaryTagId = 1 }, // Vegetarian
                    new RecipeDietaryTag { RecipeId = 4, DietaryTagId = 2 }, // Vegan
                    new RecipeDietaryTag { RecipeId = 4, DietaryTagId = 3 }, // Gluten-Free
                    // Beef Wellington tags
                    new RecipeDietaryTag { RecipeId = 5, DietaryTagId = 5 }, // High Protein
                    // Vegan Pad Thai tags
                    new RecipeDietaryTag { RecipeId = 6, DietaryTagId = 2 }, // Vegan
                    new RecipeDietaryTag { RecipeId = 6, DietaryTagId = 3 }, // Gluten-Free
                    // Gluten-Free Chocolate Cake tags
                    new RecipeDietaryTag { RecipeId = 7, DietaryTagId = 3 }, // Gluten-Free
                    new RecipeDietaryTag { RecipeId = 7, DietaryTagId = 4 }, // Dairy-Free
                    // Mediterranean Salmon tags
                    new RecipeDietaryTag { RecipeId = 8, DietaryTagId = 5 }, // High Protein
                    new RecipeDietaryTag { RecipeId = 8, DietaryTagId = 6 }, // Low-Carb
                    // Quinoa Bowl tags
                    new RecipeDietaryTag { RecipeId = 9, DietaryTagId = 1 }, // Vegetarian
                    new RecipeDietaryTag { RecipeId = 9, DietaryTagId = 2 }, // Vegan
                    new RecipeDietaryTag { RecipeId = 9, DietaryTagId = 3 }, // Gluten-Free
                    // Cauliflower Rice tags
                    new RecipeDietaryTag { RecipeId = 10, DietaryTagId = 1 }, // Vegetarian
                    new RecipeDietaryTag { RecipeId = 10, DietaryTagId = 6 } // Low-Carb
                );

            // Seed ingredients for each recipe
            modelBuilder
                .Entity<Ingredient>()
                .HasData(
                    // Ingredients for Margherita Pizza
                    new Ingredient
                    {
                        Id = 1,
                        RecipeId = 1,
                        Name = "Pizza Dough",
                        Amount = "1",
                        Unit = "ball"
                    },
                    new Ingredient
                    {
                        Id = 2,
                        RecipeId = 1,
                        Name = "Tomato Sauce",
                        Amount = "1/2",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 3,
                        RecipeId = 1,
                        Name = "Fresh Mozzarella",
                        Amount = "8",
                        Unit = "oz"
                    },
                    new Ingredient
                    {
                        Id = 4,
                        RecipeId = 1,
                        Name = "Fresh Basil",
                        Amount = "1/4",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 5,
                        RecipeId = 1,
                        Name = "Olive Oil",
                        Amount = "2",
                        Unit = "tbsp"
                    },
                    // Ingredients for Chicken Stir Fry
                    new Ingredient
                    {
                        Id = 6,
                        RecipeId = 2,
                        Name = "Chicken Breast",
                        Amount = "1",
                        Unit = "lb"
                    },
                    new Ingredient
                    {
                        Id = 7,
                        RecipeId = 2,
                        Name = "Broccoli",
                        Amount = "2",
                        Unit = "cups"
                    },
                    new Ingredient
                    {
                        Id = 8,
                        RecipeId = 2,
                        Name = "Bell Peppers",
                        Amount = "2",
                        Unit = "medium"
                    },
                    new Ingredient
                    {
                        Id = 9,
                        RecipeId = 2,
                        Name = "Soy Sauce",
                        Amount = "1/4",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 10,
                        RecipeId = 2,
                        Name = "Ginger",
                        Amount = "1",
                        Unit = "tbsp"
                    },
                    // Ingredients for Chocolate Chip Cookies
                    new Ingredient
                    {
                        Id = 11,
                        RecipeId = 3,
                        Name = "All-Purpose Flour",
                        Amount = "2",
                        Unit = "cups"
                    },
                    new Ingredient
                    {
                        Id = 12,
                        RecipeId = 3,
                        Name = "Butter",
                        Amount = "1",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 13,
                        RecipeId = 3,
                        Name = "Brown Sugar",
                        Amount = "3/4",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 14,
                        RecipeId = 3,
                        Name = "Chocolate Chips",
                        Amount = "2",
                        Unit = "cups"
                    },
                    new Ingredient
                    {
                        Id = 15,
                        RecipeId = 3,
                        Name = "Vanilla Extract",
                        Amount = "1",
                        Unit = "tsp"
                    },
                    // Ingredients for Buddha Bowl
                    new Ingredient
                    {
                        Id = 51,
                        RecipeId = 4,
                        Name = "Quinoa",
                        Amount = "1",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 52,
                        RecipeId = 4,
                        Name = "Sweet Potato",
                        Amount = "1",
                        Unit = "medium"
                    },
                    new Ingredient
                    {
                        Id = 53,
                        RecipeId = 4,
                        Name = "Chickpeas",
                        Amount = "1",
                        Unit = "can"
                    },
                    new Ingredient
                    {
                        Id = 54,
                        RecipeId = 4,
                        Name = "Kale",
                        Amount = "2",
                        Unit = "cups"
                    },
                    new Ingredient
                    {
                        Id = 55,
                        RecipeId = 4,
                        Name = "Tahini",
                        Amount = "2",
                        Unit = "tbsp"
                    },
                    // Ingredients for Beef Wellington
                    new Ingredient
                    {
                        Id = 56,
                        RecipeId = 5,
                        Name = "Beef Fillet",
                        Amount = "2",
                        Unit = "lbs"
                    },
                    new Ingredient
                    {
                        Id = 57,
                        RecipeId = 5,
                        Name = "Mushrooms",
                        Amount = "1",
                        Unit = "lb"
                    },
                    new Ingredient
                    {
                        Id = 58,
                        RecipeId = 5,
                        Name = "Puff Pastry",
                        Amount = "1",
                        Unit = "sheet"
                    },
                    new Ingredient
                    {
                        Id = 59,
                        RecipeId = 5,
                        Name = "Prosciutto",
                        Amount = "8",
                        Unit = "slices"
                    },
                    new Ingredient
                    {
                        Id = 60,
                        RecipeId = 5,
                        Name = "Dijon Mustard",
                        Amount = "2",
                        Unit = "tbsp"
                    },
                    // Ingredients for Vegan Pad Thai
                    new Ingredient
                    {
                        Id = 61,
                        RecipeId = 6,
                        Name = "Rice Noodles",
                        Amount = "8",
                        Unit = "oz"
                    },
                    new Ingredient
                    {
                        Id = 62,
                        RecipeId = 6,
                        Name = "Tofu",
                        Amount = "14",
                        Unit = "oz"
                    },
                    new Ingredient
                    {
                        Id = 63,
                        RecipeId = 6,
                        Name = "Bean Sprouts",
                        Amount = "2",
                        Unit = "cups"
                    },
                    new Ingredient
                    {
                        Id = 64,
                        RecipeId = 6,
                        Name = "Peanuts",
                        Amount = "1/2",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 65,
                        RecipeId = 6,
                        Name = "Tamarind Paste",
                        Amount = "2",
                        Unit = "tbsp"
                    },
                    // Ingredients for Gluten-Free Chocolate Cake
                    new Ingredient
                    {
                        Id = 66,
                        RecipeId = 7,
                        Name = "Almond Flour",
                        Amount = "2",
                        Unit = "cups"
                    },
                    new Ingredient
                    {
                        Id = 67,
                        RecipeId = 7,
                        Name = "Cocoa Powder",
                        Amount = "3/4",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 68,
                        RecipeId = 7,
                        Name = "Coconut Sugar",
                        Amount = "1",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 69,
                        RecipeId = 7,
                        Name = "Coconut Oil",
                        Amount = "1/2",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 70,
                        RecipeId = 7,
                        Name = "Almond Milk",
                        Amount = "1",
                        Unit = "cup"
                    },
                    // Ingredients for Mediterranean Salmon
                    new Ingredient
                    {
                        Id = 71,
                        RecipeId = 8,
                        Name = "Salmon Fillets",
                        Amount = "4",
                        Unit = "pieces"
                    },
                    new Ingredient
                    {
                        Id = 72,
                        RecipeId = 8,
                        Name = "Lemon",
                        Amount = "1",
                        Unit = "whole"
                    },
                    new Ingredient
                    {
                        Id = 73,
                        RecipeId = 8,
                        Name = "Olive Oil",
                        Amount = "2",
                        Unit = "tbsp"
                    },
                    new Ingredient
                    {
                        Id = 74,
                        RecipeId = 8,
                        Name = "Garlic",
                        Amount = "4",
                        Unit = "cloves"
                    },
                    new Ingredient
                    {
                        Id = 75,
                        RecipeId = 8,
                        Name = "Herbs",
                        Amount = "1/4",
                        Unit = "cup"
                    },
                    // Ingredients for Quinoa Bowl
                    new Ingredient
                    {
                        Id = 76,
                        RecipeId = 9,
                        Name = "Quinoa",
                        Amount = "1",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 77,
                        RecipeId = 9,
                        Name = "Avocado",
                        Amount = "1",
                        Unit = "whole"
                    },
                    new Ingredient
                    {
                        Id = 78,
                        RecipeId = 9,
                        Name = "Black Beans",
                        Amount = "1",
                        Unit = "can"
                    },
                    new Ingredient
                    {
                        Id = 79,
                        RecipeId = 9,
                        Name = "Corn",
                        Amount = "1",
                        Unit = "cup"
                    },
                    new Ingredient
                    {
                        Id = 80,
                        RecipeId = 9,
                        Name = "Lime",
                        Amount = "1",
                        Unit = "whole"
                    },
                    // Ingredients for Cauliflower Rice
                    new Ingredient
                    {
                        Id = 81,
                        RecipeId = 10,
                        Name = "Cauliflower",
                        Amount = "1",
                        Unit = "head"
                    },
                    new Ingredient
                    {
                        Id = 82,
                        RecipeId = 10,
                        Name = "Onion",
                        Amount = "1",
                        Unit = "medium"
                    },
                    new Ingredient
                    {
                        Id = 83,
                        RecipeId = 10,
                        Name = "Garlic",
                        Amount = "2",
                        Unit = "cloves"
                    },
                    new Ingredient
                    {
                        Id = 84,
                        RecipeId = 10,
                        Name = "Olive Oil",
                        Amount = "2",
                        Unit = "tbsp"
                    },
                    new Ingredient
                    {
                        Id = 85,
                        RecipeId = 10,
                        Name = "Herbs",
                        Amount = "1/4",
                        Unit = "cup"
                    }
                );
        }
    }
}

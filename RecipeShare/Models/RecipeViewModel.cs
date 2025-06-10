using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Models
{
    public class RecipeListViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? DietaryTags { get; set; }
        public string? DifficultyLevel { get; set; }
    }

    public class RecipeDetailViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Instructions { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Servings { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string>? DietaryTags { get; set; }
        public List<IngredientViewModel>? Ingredients { get; set; }
        public string? DifficultyLevel { get; set; }
    }

    public class CreateRecipeViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "Title must be between 3 and 100 characters"
        )]
        public string? Title { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public string? Instructions { get; set; }

        [Range(0, 1000, ErrorMessage = "Prep time must be between 0 and 1000 minutes")]
        public int PrepTimeMinutes { get; set; }

        [Range(
            1,
            1000,
            ErrorMessage = "Cook time must be greater than 0 and less than 1000 minutes"
        )]
        public int CookTimeMinutes { get; set; }

        [Range(1, 100)]
        public int Servings { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        public List<string>? DietaryTags { get; set; }
        public List<int>? DietaryTagIds { get; set; }
        public List<CreateIngredientViewModel>? Ingredients { get; set; }

        [Required]
        public string? DifficultyLevel { get; set; }
        public int? DifficultyLevelId { get; set; }
    }

    public class UpdateRecipeViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "Title must be between 3 and 100 characters"
        )]
        public string? Title { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public string? Instructions { get; set; }

        [Range(0, 1000, ErrorMessage = "Prep time must be between 0 and 1000 minutes")]
        public int PrepTimeMinutes { get; set; }

        [Range(
            1,
            1000,
            ErrorMessage = "Cook time must be greater than 0 and less than 1000 minutes"
        )]
        public int CookTimeMinutes { get; set; }

        [Range(1, 100)]
        public int Servings { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        public List<string>? DietaryTags { get; set; }
        public List<int>? DietaryTagIds { get; set; }
        public List<CreateIngredientViewModel>? Ingredients { get; set; }

        [Required]
        public string? DifficultyLevel { get; set; }
        public int? DifficultyLevelId { get; set; }
    }
}

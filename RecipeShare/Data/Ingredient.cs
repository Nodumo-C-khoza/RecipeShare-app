using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Data
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Amount { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Required]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}

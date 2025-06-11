using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Models
{
    public class IngredientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [StringLength(50, ErrorMessage = "Amount must be less than 50 characters")]
        public string Amount { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(50, ErrorMessage = "Unit must be less than 50 characters")]
        public string Unit { get; set; } = string.Empty;
    }

    public class CreateIngredientViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [StringLength(50, ErrorMessage = "Amount must be less than 50 characters")]
        public string Amount { get; set; } = string.Empty;

        [StringLength(20)]
        public string Unit { get; set; }
    }
}

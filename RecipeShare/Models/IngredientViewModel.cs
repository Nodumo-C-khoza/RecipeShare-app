using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Models
{
    public class IngredientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Unit { get; set; }
    }

    public class CreateIngredientViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Amount { get; set; }

        [StringLength(20)]
        public string Unit { get; set; }
    }
}

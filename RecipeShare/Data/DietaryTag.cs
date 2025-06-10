using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Data;

public class DietaryTag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DisplayName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}

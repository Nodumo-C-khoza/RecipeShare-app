namespace RecipeShare.Data
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Servings { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<Ingredient> Ingredients { get; set; }

        public int DifficultyLevelId { get; set; }

        public DifficultyLevel DifficultyLevel { get; set; } = null!;

        public ICollection<DietaryTag> DietaryTags { get; set; } = new List<DietaryTag>();
    }
}

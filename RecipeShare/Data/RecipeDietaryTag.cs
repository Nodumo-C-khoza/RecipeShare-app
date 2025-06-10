using System;

namespace RecipeShare.Data
{
    public class RecipeDietaryTag
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int DietaryTagId { get; set; }
        public DietaryTag DietaryTag { get; set; }
    }
}

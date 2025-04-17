using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public int CaloriesPerUnit { get; set; }

        // Many-to-Many relationship with Ingredients
        public ICollection<RecipexIngredient> RecipesXIngredients { get; set; }

    }

    public class IngredientDto
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public int CaloriesPerUnit { get; set; }

        public decimal Quantity { get; set; }

    }

}

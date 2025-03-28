using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class RecipexIngredient
    {
        [Key]
        public int RecipexIngredientId { get; set; } // Primary Key for the junction table

        [ForeignKey("Recipes")]
        public int RecipeId { get; set; } // Foreign Key to Recipe

        [ForeignKey("Ingredients")]
        public int IngredientId { get; set; } // Foreign Key to Ingredient

        public decimal Quantity { get; set; } // Quantity of the ingredient used in the recipe

        public string Unit { get; set; } // Measurement unit (e.g., grams, cups)

    }
}

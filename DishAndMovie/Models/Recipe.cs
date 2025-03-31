using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DishAndMovie.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }

        public string Name { get; set; }

        [ForeignKey("Origins")]
        public int OriginId { get; set; }

        public virtual Origin Origin { get; set; }

        // Many-to-Many relationship with Recipes
        public ICollection<RecipexIngredient> RecipesXIngredients { get; set; }
    }

    public class RecipeDto
    {
        public int RecipeId { get; set; }
        public string Name { get; set; }

        // Only the OriginId is needed
        public int? OriginId { get; set; }

        // Include only the OriginCountry name in the RecipeDto
        public string OriginCountry { get; set; }

        public IEnumerable<OriginDto> Origin { get; set; }
    }
}

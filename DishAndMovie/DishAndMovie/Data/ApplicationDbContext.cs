using DishAndMovie.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DishAndMovie.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //create a Origins table from the model
        public DbSet<Origin> Origins { get; set; }

        //create a Recipes table from the model
        public DbSet<Recipe> Recipes { get; set; }

        //create a Ingredients table from the model
        public DbSet<Ingredient> Ingredients { get; set; }

        //create a MealPlans table from the model
        public DbSet<MealPlan> MealPlans { get; set; }

        public DbSet<RecipexIngredient> RecipesXIngredients { get; set; }
    }
}

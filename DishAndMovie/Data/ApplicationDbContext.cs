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

        public DbSet<Movie>? Movies { get; set; }
        public DbSet<Genre>? Genres { get; set; }
        public DbSet<Review>? Reviews { get; set; }
        public DbSet<MovieGenre>? MovieGenres { get; set; }

        //create a Origins table from the model
        public DbSet<Origin> Origins { get; set; }

        //create a Recipes table from the model
        public DbSet<Recipe> Recipes { get; set; }

        //create a Ingredients table from the model
        public DbSet<Ingredient> Ingredients { get; set; }

        //create a MealPlans table from the model
        public DbSet<MealPlan> MealPlans { get; set; }

        public DbSet<RecipexIngredient> RecipesXIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship between Movies and Genres
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieID, mg.GenreID });

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieID);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreID);

            // Configure the relationship between Review and ApplicationUser
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID)
                .HasPrincipalKey(u => u.Id);
        }
    }
}

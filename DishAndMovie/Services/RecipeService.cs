using DishAndMovie.Data;
using DishAndMovie.Models;
using DishAndMovie.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DishAndMovie.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all recipes with their associated origin details
        public async Task<IEnumerable<RecipeDto>> ListRecipes()
        {
            // Fetch recipes and include the associated origin data
            var recipes = await _context.Recipes
                                        .Include(r => r.Origin) // Including Origin in the result
                                        .ToListAsync();

            // Convert to RecipeDto including both OriginId and OriginCountry
            var recipeDtos = recipes.Select(r => new RecipeDto()
            {
                RecipeId = r.RecipeId,
                Name = r.Name,
                OriginId = r.OriginId, // Include OriginId
            }).ToList();

            return recipeDtos;
        }


        // Find a specific recipe by ID
        public async Task<RecipeDto?> FindRecipe(int id)
        {
            var recipe = await _context.Recipes
                                        .Include(r => r.Origin) // Include Origin data
                                        .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return null;
            }

            // Convert to RecipeDto, including both OriginId and OriginCountry
            var recipeDto = new RecipeDto()
            {
                RecipeId = recipe.RecipeId,
                Name = recipe.Name,
                OriginId = recipe.OriginId, // Include OriginId
            };

            return recipeDto;
        }



        // Update an existing recipe
        public async Task<ServiceResponse> UpdateRecipe(int id, RecipeDto recipeDto)
        {
            ServiceResponse serviceResponse = new();

            // Find the existing recipe by ID
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Recipe not found");
                return serviceResponse;
            }

            // Update the recipe details
            recipe.Name = recipeDto.Name;
            recipe.OriginId = recipeDto.OriginId ?? recipe.OriginId; // Only use the OriginId for updates

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"Error updating recipe: {ex.Message}");
            }

            return serviceResponse;
        }


        // Add a new recipe
        public async Task<ServiceResponse> AddRecipe(RecipeDto recipeDto)
        {
            ServiceResponse serviceResponse = new();

            // Create a new Recipe entity, using only OriginId from the RecipeDto
            var recipe = new Recipe()
            {
                Name = recipeDto.Name,
                OriginId = recipeDto.OriginId ?? 0 // Only use the OriginId
            };

            // Add the new recipe to the database
            _context.Recipes.Add(recipe);

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
                serviceResponse.CreatedId = recipe.RecipeId;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"Error adding recipe: {ex.Message}");
            }

            return serviceResponse;
        }


        // Delete an existing recipe
        public async Task<ServiceResponse> DeleteRecipe(int id)
        {
            ServiceResponse serviceResponse = new();

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Recipe not found.");
                return serviceResponse;
            }

            try
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"Error deleting recipe: {ex.Message}");
            }

            return serviceResponse;
        }
    }
}

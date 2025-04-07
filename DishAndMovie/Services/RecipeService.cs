using DishAndMovie.Data;
using DishAndMovie.Models;
using DishAndMovie.Interfaces;
using Microsoft.EntityFrameworkCore;
using DishAndMovie.Data.Migrations;

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

            var movies = await _context.Movies
                .Where(r => r.OriginId == recipe.OriginId)
                .ToListAsync();

            // Convert to RecipeDto, including both OriginId and OriginCountry
            var recipeDto = new RecipeDto()
            {
                RecipeId = recipe.RecipeId,
                Name = recipe.Name,
                OriginId = recipe.OriginId,
                MoviesFromSameOrigin = movies //
            };

            return recipeDto;
        }


        // Update an existing recipe
        public async Task<ServiceResponse> UpdateRecipe(int id, RecipeDto recipeDto)
        {
            ServiceResponse serviceResponse = new();

            try
            {
                // Find the recipe by ID
                var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == id);
                if (recipe == null)
                {
                    serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                    serviceResponse.Messages.Add("Recipe not found.");
                    return serviceResponse;
                }

                // Update recipe properties
                recipe.Name = recipeDto.Name;
                recipe.OriginId = recipeDto.OriginId;

                // Save changes
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
            var serviceResponse = new ServiceResponse();

            try
            {
                // Validate input
                if (string.IsNullOrEmpty(recipeDto.Name))
                {
                    serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                    serviceResponse.Messages.Add("Recipe name is required.");
                    return serviceResponse;
                }

                // Create and add recipe
                var recipe = new Recipe
                {
                    Name = recipeDto.Name,
                    OriginId = recipeDto.OriginId
                };

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                // Return success
                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
                serviceResponse.CreatedId = recipe.RecipeId;
                serviceResponse.Messages.Add("Recipe created successfully.");
            }
            catch (Exception ex)
            {
                // Handle error
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred while creating the recipe: " + ex.Message);
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

        // Method to get all origins
        public async Task<List<OriginDto>> GetOriginsAsync()
        {
            return await _context.Origins
                .Select(o => new OriginDto
                {
                    OriginId = o.OriginId,
                    OriginCountry = o.OriginCountry
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<RecipeDto>> GetRecipesByOriginAsync(int originId)
        {
            return await _context.Recipes
                .Where(r => r.OriginId == originId)
                .Include(r => r.Origin)
                .Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    Name = r.Name,
                })
                .ToListAsync();
        }
    }
}

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

        public async Task<IEnumerable<RecipeDto>> ListRecipes(int skip, int perpage)
        {
            var recipes = await _context.Recipes
                .Include(r => r.Origin) // Include the associated Origin data
                .OrderBy(r => r.RecipeId)
                .Skip(skip)  // Skip the records based on page number
                .Take(perpage) // Take the number of records based on perpage
                .ToListAsync();

            // Map to RecipeDto including OriginId and OriginCountry
            var recipeDtos = recipes.Select(r => new RecipeDto()
            {
                RecipeId = r.RecipeId,
                Name = r.Name,
                OriginId = r.OriginId, // Include OriginId
            }).ToList();

            return recipeDtos;
        }

        public async Task<int> CountRecipes()
        {
            return await _context.Recipes.CountAsync();
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


        // Get all ingredients for a given recipe
        public async Task<IEnumerable<IngredientDto>> GetIngredientsForRecipeAsync(int recipeId)
        {
            var ingredients = await _context.RecipesXIngredients
                .Where(ri => ri.RecipeId == recipeId)
                .Join(_context.Ingredients,
                      ri => ri.IngredientId,
                      ing => ing.IngredientId,
                      (ri, ing) => new IngredientDto
                      {
                          IngredientId = ing.IngredientId,
                          Name = ing.Name,
                          Unit = ri.Unit,
                          CaloriesPerUnit = ing.CaloriesPerUnit,
                          Quantity = ri.Quantity
                      })
                .ToListAsync();

            return ingredients;
        }


        // Add a new ingredient to a recipe
        public async Task<ServiceResponse> AddIngredientToRecipeAsync(int recipeId, int ingredientId, decimal quantity)
        {
            var serviceResponse = new ServiceResponse();

            try
            {
                var recipe = await _context.Recipes.FindAsync(recipeId);
                var ingredient = await _context.Ingredients.FindAsync(ingredientId);

                // Check if the combination already exists
                var existing = await _context.RecipesXIngredients
                    .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.IngredientId == ingredientId);

                if (existing != null)
                {
                    serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                    serviceResponse.Messages.Add("Ingredient already exists in the recipe.");
                    return serviceResponse;
                }

                var recipexIngredient = new RecipexIngredient
                {
                    RecipeId = recipeId,
                    IngredientId = ingredientId,
                    Quantity = quantity,
                    Unit = ingredient.Unit // Assuming unit is stored in Ingredient
                };

                _context.RecipesXIngredients.Add(recipexIngredient);
                await _context.SaveChangesAsync();

                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
                serviceResponse.Messages.Add("Ingredient added to recipe successfully.");
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"Error adding ingredient: {ex.Message}");
            }

            return serviceResponse;
        }


        // Remove an ingredient from a recipe
        public async Task<ServiceResponse> RemoveIngredientFromRecipeAsync(int recipeId, int ingredientId)
        {
            var recipeIngredient = await _context.RecipesXIngredients
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.IngredientId == ingredientId);

            if (recipeIngredient != null)
            {
                _context.RecipesXIngredients.Remove(recipeIngredient);
                await _context.SaveChangesAsync();
            }

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Deleted,
                Messages = new List<string> { "Ingredient removed successfully." }
            };
        }


    }
}

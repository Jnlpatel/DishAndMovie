using DishAndMovie.Data;
using DishAndMovie.Models;
using Microsoft.EntityFrameworkCore;
using DishAndMovie.Interfaces;

namespace DishAndMovie.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IngredientDto>> ListIngredients(int skip, int perpage)
        {
            var ingredients = await _context.Ingredients
                .OrderBy(i => i.IngredientId)
                .Skip(skip)
                .Take(perpage)
                .ToListAsync();

            var ingredientDtos = ingredients.Select(i => new IngredientDto
            {
                IngredientId = i.IngredientId,
                Name = i.Name,
                Unit = i.Unit,
                CaloriesPerUnit = i.CaloriesPerUnit
            }).ToList();

            return ingredientDtos;
        }

        public async Task<int> CountIngredients()
        {
            return await _context.Ingredients.CountAsync();
        }


        public async Task<IngredientDto?> FindIngredient(int id)
        {
            // Fetch a single ingredient by ID
            var ingredient = await _context.Ingredients.FindAsync(id);

            // If no ingredient is found, return null
            if (ingredient == null)
            {
                return null;
            }

            // Create an instance of IngredientDto
            IngredientDto ingredientDto = new IngredientDto()
            {
                IngredientId = ingredient.IngredientId,
                Name = ingredient.Name,
                Unit = ingredient.Unit,
                CaloriesPerUnit = ingredient.CaloriesPerUnit
            };

            return ingredientDto;
        }

        public async Task<ServiceResponse> UpdateIngredient(IngredientDto ingredientDto)
        {
            ServiceResponse serviceResponse = new ServiceResponse();

            // Check if the ingredient exists
            var existingIngredient = await _context.Ingredients.FindAsync(ingredientDto.IngredientId);
            if (existingIngredient == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Ingredient not found.");
                return serviceResponse;
            }

            // Update Ingredient properties
            existingIngredient.Name = ingredientDto.Name;
            existingIngredient.Unit = ingredientDto.Unit;
            existingIngredient.CaloriesPerUnit = ingredientDto.CaloriesPerUnit;

            // Mark entity as modified
            _context.Entry(existingIngredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"An error occurred while updating the ingredient: {ex.Message}");
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"Unexpected error: {ex.Message}");
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> AddIngredient(IngredientDto ingredientDto)
        {
            ServiceResponse response = new();

            // Create a new Ingredient entity
            Ingredient ingredient = new Ingredient()
            {
                Name = ingredientDto.Name,
                Unit = ingredientDto.Unit,
                CaloriesPerUnit = ingredientDto.CaloriesPerUnit
            };

            // Add the new ingredient to the database
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            // Return success response
            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = ingredient.IngredientId;
            return response;
        }

        public async Task<ServiceResponse> DeleteIngredient(int id)
        {
            ServiceResponse response = new();

            // Find the ingredient by ID
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Ingredient not found. Cannot be deleted.");
                return response;
            }

            try
            {
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while deleting the ingredient.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }

        public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients
                .Select(i => new IngredientDto
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    Unit = i.Unit,
                    CaloriesPerUnit = i.CaloriesPerUnit
                })
                .ToListAsync();
        }


    }
}

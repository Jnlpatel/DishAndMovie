using Microsoft.EntityFrameworkCore;
using DishAndMovie.Data;
using DishAndMovie.Models;
using DishAndMovie.Interfaces;

namespace DishAndMovie.Services
{
    public class MealPlanService : IMealPlanService
    {
        private readonly ApplicationDbContext _context;

        public MealPlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MealPlanDto>> ListMealPlans(int skip, int perpage)
        {
            var mealPlans = await _context.MealPlans
                .OrderBy(mp => mp.MealPlanId)
                .Skip(skip)
                .Take(perpage)
                .ToListAsync();

            var mealPlanDtos = mealPlans.Select(mp => new MealPlanDto()
            {
                MealPlanId = mp.MealPlanId,
                Name = mp.Name,
                Date = mp.Date
            }).ToList();

            return mealPlanDtos;
        }


        public async Task<int> CountMealPlans()
        {
            return await _context.MealPlans.CountAsync();
        }


        public async Task<MealPlanDto?> FindMealPlan(int id)
        {
            // Fetch a single meal plan by ID
            var mealPlan = await _context.MealPlans.FindAsync(id);

            // If no meal plan is found, return null
            if (mealPlan == null)
            {
                return null;
            }

            // Create an instance of MealPlanDto
            MealPlanDto mealPlanDto = new MealPlanDto()
            {
                MealPlanId = mealPlan.MealPlanId,
                Name = mealPlan.Name,
                Date = mealPlan.Date
            };

            return mealPlanDto;
        }

        public async Task<ServiceResponse> UpdateMealPlan(MealPlanDto mealPlanDto)
        {
            ServiceResponse serviceResponse = new();

            // Check if the meal plan exists
            var existingMealPlan = await _context.MealPlans.FindAsync(mealPlanDto.MealPlanId);
            if (existingMealPlan == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Meal Plan not found.");
                return serviceResponse;
            }

            // Update Meal Plan properties
            existingMealPlan.Name = mealPlanDto.Name;
            existingMealPlan.Date = mealPlanDto.Date;

            // Mark entity as modified
            _context.Entry(existingMealPlan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"An error occurred while updating the mealplan: {ex.Message}");
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"Unexpected error: {ex.Message}");
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> AddMealPlan(MealPlanDto mealPlanDto)
        {
            ServiceResponse response = new();

            // Create a new MealPlan entity
            MealPlan mealPlan = new MealPlan()
            {
                Name = mealPlanDto.Name,
                Date = mealPlanDto.Date
            };

            // Add the new meal plan to the database
            _context.MealPlans.Add(mealPlan);
            await _context.SaveChangesAsync();

            // Return success response
            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = mealPlan.MealPlanId;
            return response;
        }

        public async Task<ServiceResponse> DeleteMealPlan(int id)
        {
            ServiceResponse response = new();

            // Find the meal plan by ID
            var mealPlan = await _context.MealPlans.FindAsync(id);
            if (mealPlan == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Meal Plan not found. Cannot be deleted.");
                return response;
            }

            try
            {
                _context.MealPlans.Remove(mealPlan);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while deleting the meal plan.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;
        }


    }
}

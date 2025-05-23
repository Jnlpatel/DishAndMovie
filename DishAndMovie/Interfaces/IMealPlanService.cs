﻿using DishAndMovie.Models;

namespace DishAndMovie.Interfaces
{
    public interface IMealPlanService
    {
        // Base CRUD operations
        Task<IEnumerable<MealPlanDto>> ListMealPlans(int skip, int perpage);
        Task<MealPlanDto?> FindMealPlan(int id);
        Task<ServiceResponse> AddMealPlan(MealPlanDto mealPlanDto);
        Task<ServiceResponse> UpdateMealPlan(MealPlanDto mealPlanDto);
        Task<ServiceResponse> DeleteMealPlan(int id);

        Task<int> CountMealPlans();
    }
}

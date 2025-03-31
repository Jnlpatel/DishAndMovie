using DishAndMovie.Models;

namespace DishAndMovie.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<RecipeDto>> ListRecipes();

        Task<RecipeDto?> FindRecipe(int id);

        Task<ServiceResponse> UpdateRecipe(int id, RecipeDto recipeDto);

        Task<ServiceResponse> AddRecipe(RecipeDto recipeDto);

        Task<ServiceResponse> DeleteRecipe(int id);
    }
}

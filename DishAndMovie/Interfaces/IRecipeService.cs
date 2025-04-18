using DishAndMovie.Models;

namespace DishAndMovie.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<RecipeDto>> ListRecipes(int skip, int perpage);

        Task<RecipeDto?> FindRecipe(int id);

        Task<ServiceResponse> UpdateRecipe(int id, RecipeDto recipeDto);

        Task<ServiceResponse> AddRecipe(RecipeDto recipeDto);

        Task<ServiceResponse> DeleteRecipe(int id);

        Task<List<OriginDto>> GetOriginsAsync();  // Retrieve list of origins
        Task<IEnumerable<RecipeDto>> GetRecipesByOriginAsync(int originId);

        // Methods for handling ingredients in a recipe
        Task<IEnumerable<IngredientDto>> GetIngredientsForRecipeAsync(int recipeId);
        Task<ServiceResponse> AddIngredientToRecipeAsync(int recipeId, int ingredientId, decimal quantity);
        Task<ServiceResponse> RemoveIngredientFromRecipeAsync(int recipeId, int ingredientId);

        Task<int> CountRecipes();

    }


}

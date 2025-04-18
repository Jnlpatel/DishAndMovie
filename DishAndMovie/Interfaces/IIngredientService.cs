using DishAndMovie.Models;

namespace DishAndMovie.Interfaces
{
    public interface IIngredientService
    {
        // Base CRUD operations
        Task<IEnumerable<IngredientDto>> ListIngredients(int skip, int perpage);

        Task<IngredientDto?> FindIngredient(int id);

        Task<ServiceResponse> UpdateIngredient(IngredientDto ingredientDto);

        Task<ServiceResponse> AddIngredient(IngredientDto ingredientDto);

        Task<ServiceResponse> DeleteIngredient(int id);

        Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync();

        Task<int> CountIngredients();
    }
}

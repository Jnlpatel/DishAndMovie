namespace DishAndMovie.Models.ViewModels
{
    public class IngredientList
    {
        public IEnumerable<IngredientDto?> Ingredients { get; set; }
        public int Page { get; set; }
        public int MaxPage { get; set; }
        public int PerPage { get; set; }
    }
}

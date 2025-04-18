namespace DishAndMovie.Models.ViewModels
{
    public class RecipeList
    {
        // This ViewModel is the structure needed for us to render RecipePage/List.cshtml

        public IEnumerable<RecipeDto> Recipes { get; set; }

        public int Page { get; set; }

        public int MaxPage { get; set; }
    }
}

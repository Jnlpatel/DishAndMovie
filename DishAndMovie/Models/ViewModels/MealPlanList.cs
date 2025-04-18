namespace DishAndMovie.Models.ViewModels
{
    public class MealPlanList
    {
        // This ViewModel is the structure needed for us to render MealPlanPage/List.cshtml

        public IEnumerable<MealPlanDto> MealPlans { get; set; }

        public int Page { get; set; }

        public int MaxPage { get; set; }
    }
}

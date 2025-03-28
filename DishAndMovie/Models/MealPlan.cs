using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class MealPlan
    {
        [Key]
        public int MealPlanId { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }
    }
}

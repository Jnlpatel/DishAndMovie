namespace DishAndMovie.Models.ViewModels
{
    public class OriginList
    {
        public IEnumerable<OriginDto?> Origins { get; set; }
        public int Page { get; set; }
        public int MaxPage { get; set; } 
    }
}

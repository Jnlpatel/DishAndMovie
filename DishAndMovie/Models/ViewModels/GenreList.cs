namespace DishAndMovie.Models.ViewModels
{
    public class GenreList
    {
        public IEnumerable<GenreDto> Genres { get; set; }
        public int Page { get; set; }
        public int MaxPage { get; set; }
    }

}

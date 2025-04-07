using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class Origin
    {
        [Key]
        public int OriginId { get; set; }

        public string OriginCountry { get; set; }

        // One-to-Many relationships
        public ICollection<Movie>? Movies { get; set; } = new List<Movie>();
        public ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();

    }
    public class OriginDto
    {
        public int OriginId { get; set; }
        public string OriginCountry { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class Origin
    {
        [Key]
        public int OriginId { get; set; }

        public string OriginCountry { get; set; }

        // One-to-Many relationships
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();

    }
    public class OriginDto
    {
        public int OriginId { get; set; }
        public string OriginCountry { get; set; }
    }

}

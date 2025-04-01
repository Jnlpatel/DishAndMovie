using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class MovieGenre
    {
        [Required]
        public int MovieID { get; set; }

        [ForeignKey("MovieID")]
        public Movie? Movie { get; set; }

        [Required]
        public int GenreID { get; set; }

        [ForeignKey("GenreID")]
        public Genre? Genre { get; set; }
    }
}

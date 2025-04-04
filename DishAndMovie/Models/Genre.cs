using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class Genre
    {
        [Key]
        public int GenreID { get; set; }

        [Required(ErrorMessage = "The Genre Name field is required.")]
        [StringLength(100, ErrorMessage = "The Genre Name cannot exceed 100 characters.")]
        [Display(Name = "Genre Name")]
        public string? Name { get; set; }

        // Navigation property:
        // Many-to-many relationship with Movies (through MovieGenres)
        public ICollection<MovieGenre>? MovieGenres { get; set; }
    }
    public class GenreDto
    {
        public int GenreID { get; set; }
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DishAndMovie.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        [StringLength(255, ErrorMessage = "The Title cannot exceed 255 characters.")]
        [Display(Name = "Movie Title")]
        public string? Title { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The Release Date field is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }
        public string PosterURL { get; set; }
        public string Director { get; set; }

        [ForeignKey("Origins")]
        public int OriginId { get; set; }
        public Origin? Origin { get; set; }


        // Navigation properties:
        // One-to-many relationship with Reviews
        public ICollection<Review>? Reviews { get; set; }

        // Many-to-many relationship with Genres (through MovieGenres)
        public ICollection<MovieGenre>? MovieGenres { get; set; }
    }
}

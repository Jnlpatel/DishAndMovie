using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DishAndMovie.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Required(ErrorMessage = "The Movie field is required.")]
        [Display(Name = "Movie")]
        public int MovieID { get; set; }  // Foreign Key
        [ForeignKey("MovieID")]
        public Movie? Movie { get; set; }  // Navigation property

        [Required(ErrorMessage = "The User field is required.")]
        [Display(Name = "User")]
        public string? UserID { get; set; } // Foreign Key, links to AspNetUsers table
        [ForeignKey("UserID")]
        public ApplicationUser? User { get; set; } // Navigation property

        [Required(ErrorMessage = "The ReviewText field is required.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "ReviewText")]
        public string? ReviewText { get; set; }

        [Required(ErrorMessage = "The Rating field is required.")]
        [Range(1, 5, ErrorMessage = "The Rating must be between 1 and 5 stars.")]
        [Display(Name = "Rating (1-5 stars)")]
        public int Rating { get; set; }

        [Display(Name = "Review Date")]
        public DateTime ReviewDate { get; set; } = DateTime.Now; // Default value
    }
}

using Microsoft.AspNetCore.Identity;

namespace DishAndMovie.Models
{
    public class ApplicationUser : IdentityUser  // Correct inheritance
    {
        public int UserID { get; set; }

        public string? UserName { get; set; }
        // Navigation property: Reviews (one-to-many)
        public ICollection<Review>? Reviews { get; set; }
    }
}

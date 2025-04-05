using DishAndMovie.Data;
using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.EntityFrameworkCore;

namespace DishAndMovie.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> ListReviewsByMovie(int MovieID)
        {
            Console.WriteLine(MovieID);
            return await _context.Reviews
                .Where(r => r.MovieID == MovieID)
                .Select(r => new ReviewDto
                {
                    ReviewID = r.ReviewID,
                    MovieID = r.MovieID,
                    UserID = r.UserID,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    ReviewDate = r.ReviewDate  // Ensure the date is set to current date and time

        }).ToListAsync();
        }

        public async Task<ReviewDto?> FindReview(int id)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewID == id);

            if (review == null) return null;

            return new ReviewDto
            {
                ReviewID = review.ReviewID,
                MovieID = review.MovieID,
                UserID = review.UserID,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                ReviewDate = review.ReviewDate
            };
        }

        public async Task<ServiceResponse> AddReview(ReviewDto reviewDto)
        {
            var review = new Review
            {
                MovieID = reviewDto.MovieID,
                UserID = reviewDto.UserID,
                Rating = reviewDto.Rating,
                ReviewText = reviewDto.ReviewText,
                ReviewDate = DateTime.Now  // Set ReviewDate to current server time
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Created,
                CreatedId = review.ReviewID,
                Messages = new List<string> { "Review added successfully." }
            };
        }

        public async Task<ServiceResponse> UpdateReview(ReviewDto reviewDto)
        {
            var review = await _context.Reviews.FindAsync(reviewDto.ReviewID);
            if (review == null)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.NotFound,
                    Messages = new List<string> { "Review not found." }
                };
            }

            review.Rating = reviewDto.Rating;
            review.ReviewText = reviewDto.ReviewText;

            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Updated,
                Messages = new List<string> { "Review updated successfully." }
            };
        }

        public async Task<ServiceResponse> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.NotFound,
                    Messages = new List<string> { "Review not found." }
                };
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Deleted,
                Messages = new List<string> { "Review deleted successfully." }
            };
        }
    }
}

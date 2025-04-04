using DishAndMovie.Models;

namespace DishAndMovie.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> ListReviewsByMovie(int movieId);
        Task<ReviewDto?> FindReview(int id);
        Task<ServiceResponse> AddReview(ReviewDto reviewDto);
        Task<ServiceResponse> UpdateReview(ReviewDto reviewDto);
        Task<ServiceResponse> DeleteReview(int id);
    }
}

using DishAndMovie.Models;

namespace DishAndMovie.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDto>> ListMovies();
        Task<Movie> CreateMovieAsync(Movie movie);
        Task AddMovieGenresAsync(int movieId, List<int> genreIds);
        Task<MovieDto?> FindMovie(int id);
        Task<ServiceResponse> UpdateMovie(MovieDto movieDto);
        Task<ServiceResponse> AddMovie(MovieDto movieDto);
        Task<ServiceResponse> DeleteMovie(int id);
        Task<List<GenreDto>> GetGenresAsync();  // Retrieve list of genres
        Task<List<OriginDto>> GetOriginsAsync();  // Retrieve list of origins

    }
}

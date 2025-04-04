using DishAndMovie.Models;

namespace DishAndMovie.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreDto>> ListGenres();
        Task<GenreDto?> FindGenre(int id);
        Task<ServiceResponse> UpdateGenre(GenreDto genreDto);
        Task<ServiceResponse> AddGenre(GenreDto genreDto);
        Task<ServiceResponse> DeleteGenre(int id);
    }
}

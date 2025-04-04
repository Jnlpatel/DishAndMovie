using DishAndMovie.Data;
using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.EntityFrameworkCore;

namespace DishAndMovie.Services
{
    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext _context;

        public GenreService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GenreDto>> ListGenres()
        {
            return await _context.Genres
                .Select(g => new GenreDto
                {
                    GenreID = g.GenreID,
                    Name = g.Name
                }).ToListAsync();
        }

        public async Task<GenreDto?> FindGenre(int id)
        {
            var genre = await _context.Genres
                .FirstOrDefaultAsync(g => g.GenreID == id);

            if (genre == null) return null;

            return new GenreDto
            {
                GenreID = genre.GenreID,
                Name = genre.Name
            };
        }

        public async Task<ServiceResponse> AddGenre(GenreDto genreDto)
        {
            var genre = new Genre
            {
                Name = genreDto.Name
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Created,
                CreatedId = genre.GenreID,
                Messages = new List<string> { "Genre added successfully." }
            };
        }

        public async Task<ServiceResponse> UpdateGenre(GenreDto genreDto)
        {
            // Find the genre in the database by ID
            var genre = await _context.Genres.FindAsync(genreDto.GenreID);
            if (genre == null)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.NotFound,
                    Messages = new List<string> { "Genre not found." }
                };
            }

            // Update the genre's name
            genre.Name = genreDto.Name;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return success response
            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Updated,
                Messages = new List<string> { "Genre updated successfully." }
            };
        }




        public async Task<ServiceResponse> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.NotFound,
                    Messages = new List<string> { "Genre not found." }
                };
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Deleted,
                Messages = new List<string> { "Genre deleted successfully." }
            };
        }
    }
}

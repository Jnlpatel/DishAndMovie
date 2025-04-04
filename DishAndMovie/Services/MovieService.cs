using DishAndMovie.Data;
using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.EntityFrameworkCore;

namespace DishAndMovie.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;

        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovieDto>> ListMovies()
        {
            return await _context.Movies
                .Include(m => m.Origin)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Select(m => new MovieDto
                {
                    MovieID = m.MovieID,
                    Title = m.Title,
                    Description = m.Description,
                    ReleaseDate = m.ReleaseDate,
                    PosterURL = m.PosterURL,
                    Director = m.Director,
                    OriginId = m.OriginId,
                    OriginCountry = m.Origin.OriginCountry
                }).ToListAsync();
        }

        // Find a movie by ID (if needed for editing or retrieving specific data)
        public async Task<MovieDto> FindMovie(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.Origin)
                .FirstOrDefaultAsync(m => m.MovieID == id);

            if (movie == null)
                return null;

            return new MovieDto
            {
                MovieID = movie.MovieID,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                PosterURL = movie.PosterURL,
                Director = movie.Director,
                OriginId = movie.OriginId,
                OriginCountry = movie.Origin.OriginCountry,
                GenreIds = movie.MovieGenres.Select(mg => mg.GenreID).ToList()
            };
        }

        public async Task<ServiceResponse> AddMovie(MovieDto movieDto)
        {
            var response = new ServiceResponse();

            try
            {
                // Step 1: Create a new Movie from the MovieDto
                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Description = movieDto.Description,
                    ReleaseDate = movieDto.ReleaseDate,
                    PosterURL = movieDto.PosterURL,
                    Director = movieDto.Director,
                    OriginId = movieDto.OriginId
                };

                // Add the movie to the database
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                // Step 2: Add the MovieGenres entries to the database
                if (movieDto.GenreIds != null && movieDto.GenreIds.Any())
                {
                    foreach (var genreId in movieDto.GenreIds)
                    {
                        var movieGenre = new MovieGenre
                        {
                            MovieID = movie.MovieID,
                            GenreID = genreId
                        };
                        _context.MovieGenres.Add(movieGenre);
                    }

                    await _context.SaveChangesAsync();
                }

                // Step 3: Return a response indicating that the movie was created
                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = movie.MovieID;

            }
            catch (Exception ex)
            {
                // Handle error and return the error message
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while creating the movie: " + ex.Message);
            }

            return response;
        }
    


    public async Task<ServiceResponse> UpdateMovie(MovieDto movieDto)
        {
            var movie = await _context.Movies.FindAsync(movieDto.MovieID);
            if (movie == null)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.NotFound,
                    Messages = new List<string> { "Movie not found." }
                };
            }

            movie.Title = movieDto.Title;
            movie.Description = movieDto.Description;
            movie.ReleaseDate = movieDto.ReleaseDate;
            movie.PosterURL = movieDto.PosterURL;
            movie.Director = movieDto.Director;
            movie.OriginId = movieDto.OriginId;

            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Updated,
                Messages = new List<string> { "Movie updated successfully." }
            };
        }

        public async Task<ServiceResponse> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return new ServiceResponse
                {
                    Status = ServiceResponse.ServiceStatus.NotFound,
                    Messages = new List<string> { "Movie not found." }
                };
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Status = ServiceResponse.ServiceStatus.Deleted,
                Messages = new List<string> { "Movie deleted successfully." }
            };
        }

        // Create a new movie
        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        // Add genre to the movie
        public async Task AddMovieGenresAsync(int movieId, List<int> genreIds)
        {
            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                throw new ArgumentException("Movie not found");
            }

            foreach (var genreId in genreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre != null)
                {
                    _context.MovieGenres.Add(new MovieGenre
                    {
                        MovieID = movieId,
                        GenreID = genreId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        // Method to get all genres
        public async Task<List<GenreDto>> GetGenresAsync()
        {
            return await _context.Genres
                                 .Select(g => new GenreDto { GenreID = g.GenreID, Name = g.Name })
                                 .ToListAsync();
        }

        // Method to get all origins
        public async Task<List<OriginDto>> GetOriginsAsync()
        {
            return await _context.Origins
                .Select(o => new OriginDto
                {
                    OriginId = o.OriginId,
                    OriginCountry = o.OriginCountry
                })
                .ToListAsync();
        }
    }
}

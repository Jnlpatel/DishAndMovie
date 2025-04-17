using DishAndMovie.Data;
using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.EntityFrameworkCore;

namespace DishAndMovie.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;


        public MovieService(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
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

            var recipes = await _context.Recipes
            .Where(r => r.OriginId == movie.OriginId)
            .ToListAsync();

            return new MovieDto
            {
                MovieID = movie.MovieID,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                PosterURL = movie.PosterURL,
                Director = movie.Director,
                OriginId = movie.OriginId,
                GenreIds = movie.MovieGenres.Select(mg => mg.GenreID).ToList(),
                GenreNames = movie.MovieGenres?.Select(g => g.Genre.Name).ToList(),
                RecipesFromSameOrigin = recipes

            };
        }

        public async Task<ServiceResponse> AddMovie(MovieDto movieDto, IFormFile posterImage = null)
        {
            var response = new ServiceResponse();
            Console.WriteLine($"AddMovie called with Title: {movieDto.Title}");
            Console.WriteLine($"PosterImage: {(posterImage != null ? $"{posterImage.FileName} ({posterImage.Length} bytes)" : "null")}");


            try
            {
                // Validate input
                if (string.IsNullOrEmpty(movieDto.Title))
                {
                    Console.WriteLine("Validation failed: Title is required");
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    response.Messages.Add("Movie title is required.");
                    return response;
                }

                // Handle image upload
                string posterUrl = null;
                if (posterImage != null && posterImage.Length > 0)
                {
                    Console.WriteLine("Processing poster image upload");
                    posterUrl = await _fileService.SaveImageAsync(posterImage);
                    Console.WriteLine($"Image saved at: {posterUrl}");
                }

                // Create movie entity
                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Description = movieDto.Description,
                    ReleaseDate = movieDto.ReleaseDate,
                    PosterURL = posterUrl ?? movieDto.PosterURL ?? string.Empty,
                    Director = movieDto.Director ?? string.Empty,
                    OriginId = movieDto.OriginId
                };

                Console.WriteLine("Adding movie to context");
                _context.Movies.Add(movie);

                Console.WriteLine("Saving changes to database");
                await _context.SaveChangesAsync();
                Console.WriteLine($"Movie saved with ID: {movie.MovieID}");

                // Handle genres
                if (movieDto.GenreIds != null && movieDto.GenreIds.Any())
                {
                    Console.WriteLine($"Processing {movieDto.GenreIds.Count} genres");
                    var existingGenreIds = await _context.Genres
                        .Where(g => movieDto.GenreIds.Contains(g.GenreID))
                        .Select(g => g.GenreID)
                        .ToListAsync();

                    Console.WriteLine($"Found {existingGenreIds.Count} existing genres");

                    foreach (var genreId in existingGenreIds)
                    {
                        _context.MovieGenres.Add(new MovieGenre
                        {
                            MovieID = movie.MovieID,
                            GenreID = genreId
                        });
                    }

                    await _context.SaveChangesAsync();
                    Console.WriteLine("Genres saved successfully");
                }

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = movie.MovieID;
                response.Messages.Add("Movie created successfully.");
                Console.WriteLine("Movie creation completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddMovie: {ex.ToString()}");
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while creating the movie: " + ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> UpdateMovie(
    int id,
    MovieDto movieDto,
    IFormFile posterImage = null,
    bool removeImage = false)
        {
            var response = new ServiceResponse();

            Console.WriteLine($"Updating movie ID: {id}");
            Console.WriteLine($"New image: {posterImage?.FileName ?? "null"}, Remove: {removeImage}");

            try
            {
                var movie = await _context.Movies
                    .Include(m => m.MovieGenres)
                    .FirstOrDefaultAsync(m => m.MovieID == id);

                if (movie == null)
                {
                    response.Status = ServiceResponse.ServiceStatus.NotFound;
                    response.Messages.Add("Movie not found");
                    return response;
                }

                // Handle image updates
                if (removeImage && !string.IsNullOrEmpty(movie.PosterURL))
                {
                    Console.WriteLine($"Removing existing image: {movie.PosterURL}");
                    _fileService.DeleteImage(movie.PosterURL);
                    movie.PosterURL = null;
                }
                else if (posterImage != null && posterImage.Length > 0)
                {
                    Console.WriteLine("Uploading new image");
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(movie.PosterURL))
                    {
                        _fileService.DeleteImage(movie.PosterURL);
                    }

                    // Save new image
                    movie.PosterURL = await _fileService.SaveImageAsync(posterImage);
                    Console.WriteLine($"New image path: {movie.PosterURL}");
                }

                // Update other properties
                movie.Title = movieDto.Title;
                movie.Description = movieDto.Description;
                movie.ReleaseDate = movieDto.ReleaseDate;
                movie.Director = movieDto.Director;
                movie.OriginId = movieDto.OriginId;

                // Update genres
                if (movieDto.GenreIds != null)
                {
                    Console.WriteLine("Updating genres...");

                    // Remove existing genres
                    var existingGenres = movie.MovieGenres.ToList();
                    _context.MovieGenres.RemoveRange(existingGenres);

                    // Add new genres
                    foreach (var genreId in movieDto.GenreIds)
                    {
                        if (await _context.Genres.AnyAsync(g => g.GenreID == genreId))
                        {
                            _context.MovieGenres.Add(new MovieGenre
                            {
                                MovieID = id,
                                GenreID = genreId
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("Movie updated successfully");

                response.Status = ServiceResponse.ServiceStatus.Updated;
                response.Messages.Add("Movie updated successfully");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Database error occurred");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex}");
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An unexpected error occurred");
            }

            return response;
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

        public async Task<IEnumerable<MovieDto>> GetMoviesByOriginAsync(int originId)
        {
            return await _context.Movies
                .Where(m => m.OriginId == originId)
                .Include(m => m.Origin)
                .Select(m => new MovieDto
                {
                    MovieID = m.MovieID,
                    Title = m.Title,
                    Director = m.Director,
                    ReleaseDate = m.ReleaseDate,
                    PosterURL = m.PosterURL,
                })
                .ToListAsync();
        }
        
    }
}

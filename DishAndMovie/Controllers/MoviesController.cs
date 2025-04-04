using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Mvc;

namespace DishAndMovie.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IReviewService _reviewService;

        public MoviesController(IMovieService movieService, IReviewService reviewService)
        {
            _movieService = movieService;
            _reviewService = reviewService;
        }

        /// <summary>
        /// Returns a list of all movies.
        /// </summary>
        /// <returns>A list of MovieDto objects.</returns>
        /// <example>
        /// GET: api/Movies
        /// Returns a list of all movies with details like title, release date, etc.
        /// </example>
        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieService.ListMovies();
            return Ok(movies);
        }

        /// <summary>
        /// Returns a single movie by ID.
        /// </summary>
        /// <param name="id">The ID of the movie.</param>
        /// <returns>A MovieDto object with details of the movie.</returns>
        /// <example>
        /// GET: api/Movies/{id}
        /// Returns the movie details by ID.
        /// </example>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _movieService.FindMovie(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        /// <summary>
        /// Adds a new movie.
        /// </summary>
        /// <param name="movieDto">The new movie details.</param>
        /// <returns>201 Created or 400 Bad Request depending on the success of the creation.</returns>
        /// <example>
        /// POST: api/Movies
        /// Body: { "Title": "Inception", "ReleaseDate": "2010-07-16", "Director": "Christopher Nolan", "PosterURL": "...", "OriginCountry": "USA" }
        /// </example>
        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] MovieDto movieDto)
        {
            var response = await _movieService.AddMovie(movieDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return BadRequest(response.Messages);
            }
            return CreatedAtAction(nameof(GetMovie), new { id = response.CreatedId }, response.Messages);
        }

        /// <summary>
        /// Updates an existing movie by ID.
        /// </summary>
        /// <param name="id">The ID of the movie to update.</param>
        /// <param name="movieDto">The updated movie details.</param>
        /// <returns>200 OK or 404 Not Found based on the success of the update.</returns>
        /// <example>
        /// PUT: api/Movies/{id}
        /// Body: { "Title": "Inception Updated", "ReleaseDate": "2010-07-16", "Director": "Christopher Nolan", "PosterURL": "updatedPoster.jpg", "OriginCountry": "USA" }
        /// </example>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieDto movieDto)
        {
            if (id != movieDto.MovieID)
            {
                return BadRequest("Movie ID mismatch.");
            }

            var response = await _movieService.UpdateMovie(movieDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return NotFound(response.Messages);
            }
            return Ok(response.Messages);
        }

        /// <summary>
        /// Deletes a movie by ID.
        /// </summary>
        /// <param name="id">The ID of the movie to delete.</param>
        /// <returns>200 OK or 404 Not Found depending on the success of the deletion.</returns>
        /// <example>
        /// DELETE: api/Movies/{id}
        /// Deletes the movie with the given ID.
        /// </example>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var response = await _movieService.DeleteMovie(id);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return NotFound(response.Messages);
            }
            return Ok(response.Messages);
        }

        /// <summary>
        /// Returns a list of all reviews for a specific movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie to get reviews for.</param>
        /// <returns>A list of ReviewDto objects for the specified movie.</returns>
        /// <example>
        /// GET: api/Movies/{movieId}/reviews
        /// Returns a list of reviews for the movie with the given ID.
        /// </example>
        [HttpGet("{movieId}/reviews")]
        public async Task<IActionResult> GetReviewsForMovie(int movieId)
        {
            var reviews = await _reviewService.ListReviewsByMovie(movieId);
            return Ok(reviews);
        }

        /// <summary>
        /// Adds a review for a specific movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie to add the review to.</param>
        /// <param name="reviewDto">The review details to be added.</param>
        /// <returns>201 Created or 400 Bad Request depending on the success of the review creation.</returns>
        /// <example>
        /// POST: api/Movies/{movieId}/reviews
        /// Body: { "ReviewText": "Great movie!", "Rating": 5, "UserID": "user123" }
        /// </example>
        [HttpPost("{movieId}/reviews")]
        public async Task<IActionResult> AddReviewToMovie(int movieId, [FromBody] ReviewDto reviewDto)
        {
            reviewDto.MovieID = movieId;
            var response = await _reviewService.AddReview(reviewDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return BadRequest(response.Messages);
            }
            return CreatedAtAction(nameof(GetReviewsForMovie), new { movieId = movieId }, response.Messages);
        }

        /// <summary>
        /// Updates a review for a specific movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie the review belongs to.</param>
        /// <param name="reviewId">The ID of the review to update.</param>
        /// <param name="reviewDto">The updated review details.</param>
        /// <returns>200 OK or 404 Not Found depending on the success of the update.</returns>
        /// <example>
        /// PUT: api/Movies/{movieId}/reviews/{reviewId}
        /// Body: { "ReviewText": "Amazing movie!", "Rating": 5, "UserID": "user123" }
        /// </example>
        [HttpPut("{movieId}/reviews/{reviewId}")]
        public async Task<IActionResult> UpdateReview(int movieId, int reviewId, [FromBody] ReviewDto reviewDto)
        {
            if (reviewId != reviewDto.ReviewID || movieId != reviewDto.MovieID)
            {
                return BadRequest("Review ID or Movie ID mismatch.");
            }

            var response = await _reviewService.UpdateReview(reviewDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return NotFound(response.Messages);
            }
            return Ok(response.Messages);
        }

        /// <summary>
        /// Deletes a review for a specific movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie the review belongs to.</param>
        /// <param name="reviewId">The ID of the review to delete.</param>
        /// <returns>200 OK or 404 Not Found depending on the success of the deletion.</returns>
        /// <example>
        /// DELETE: api/Movies/{movieId}/reviews/{reviewId}
        /// Deletes the review with the given review ID for the specified movie.
        /// </example>
        [HttpDelete("{movieId}/reviews/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int movieId, int reviewId)
        {
            var response = await _reviewService.DeleteReview(reviewId);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return NotFound(response.Messages);
            }
            return Ok(response.Messages);
        }
    }
}

using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DishAndMovie.Controllers
{
    public class MoviesPageController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<MoviesPageController> _logger;

        public MoviesPageController(IMovieService movieService, IReviewService reviewService, ILogger<MoviesPageController> logger)
        {
            _movieService = movieService;
            _reviewService = reviewService;
            _logger = logger;
        }

        /// <summary>
        /// Displays a list of all movies.
        /// </summary>
        /// <returns>A view showing all movies.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.ListMovies();
            if (movies == null)
            {
                _logger.LogWarning("No movies found.");
                return View("Error");  // An error view can be shown if no movies are available.
            }
            return View(movies);  // Pass the list of movies to the view.
        }

        /// <summary>
        /// Displays the details of a single movie.
        /// </summary>
        /// <param name="id">The ID of the movie.</param>
        /// <returns>A view showing the details of the movie.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.FindMovie(id);
            if (movie == null)
            {
                _logger.LogWarning($"Movie with ID {id} not found.");
                return View("Error");  // Show an error page if the movie is not found.
            }
            return View(movie);  // Show the details of the movie.
        }

        /// <summary>
        /// Displays the form for adding a new movie.
        /// </summary>
        /// <returns>A view to create a new movie.</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Retrieve list of genres and origins (assuming you have a service for these)
            ViewBag.Genres = await _movieService.GetGenresAsync();  // Fetch genres
            ViewBag.Origins = await _movieService.GetOriginsAsync();  // Fetch origins

          
            // Return the view with the initial model (MovieDto) to bind the form fields
            return View(new MovieDto());
        }

        /// <summary>
        /// Handles the form submission to create a new movie.
        /// </summary>
        /// <param name="movieDto">The movie details entered by the user.</param>
        /// <returns>Redirects to the movie details page if successful, otherwise shows validation errors.</returns>
        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(MovieDto movieDto)
        {
            if (ModelState.IsValid)
            {
                // Call the service to add the movie and genres
                var response = await _movieService.AddMovie(movieDto);

                if (response.Status == ServiceResponse.ServiceStatus.Created)
                {
                    // Redirect to the movie index page after successful creation
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // If there was an error, return the error message to the view
                    ModelState.AddModelError("", "An error occurred while creating the movie: " + string.Join(", ", response.Messages));
                }
            }

            // If the model is invalid, return the form with current data
            // Load genres and origins for dropdown lists in the view
            ViewBag.Genres = await _movieService.GetGenresAsync();
            ViewBag.Origins = await _movieService.GetOriginsAsync();
            return View(movieDto);
        }



        /// <summary>
        /// Displays the form for editing an existing movie.
        /// </summary>
        /// <param name="id">The ID of the movie to edit.</param>
        /// <returns>A view with the movie details to edit.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieService.FindMovie(id);
            if (movie == null)
            {
                _logger.LogWarning($"Movie with ID {id} not found.");
                return View("Error");
            }
            return View(movie);  // Pass the movie to the view for editing.
        }

        /// <summary>
        /// Handles the form submission to update an existing movie.
        /// </summary>
        /// <param name="id">The ID of the movie to update.</param>
        /// <param name="movieDto">The updated movie details.</param>
        /// <returns>Redirects to the movie details page if successful, otherwise shows validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieDto movieDto)
        {
            if (id != movieDto.MovieID)
            {
                _logger.LogError("Movie ID mismatch.");
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var response = await _movieService.UpdateMovie(movieDto);
                if (response.Status == ServiceResponse.ServiceStatus.Updated)
                {
                    return RedirectToAction("Details", new { id = movieDto.MovieID });
                }
                else
                {
                    _logger.LogError($"Error updating movie: {string.Join(", ", response.Messages)}");
                    ModelState.AddModelError("", string.Join(", ", response.Messages));
                }
            }
            return View(movieDto);  // Return the view with validation errors if any.
        }

        /// <summary>
        /// Deletes a movie by ID.
        /// </summary>
        /// <param name="id">The ID of the movie to delete.</param>
        /// <returns>Redirects to the movies list page after deletion.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _movieService.DeleteMovie(id);
            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError($"Error deleting movie with ID {id}: {string.Join(", ", response.Messages)}");
                return View("Error");
            }
        }

        /// <summary>
        /// Displays reviews for a movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie.</param>
        /// <returns>A view with the movie's reviews.</returns>
        [HttpGet]
        public async Task<IActionResult> Reviews(int movieId)
        {
            var reviews = await _reviewService.ListReviewsByMovie(movieId);
            if (reviews == null)
            {
                _logger.LogWarning($"No reviews found for movie ID {movieId}.");
                return View("Error");
            }
            return View(reviews);
        }

        /// <summary>
        /// Adds a review for a movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie to review.</param>
        /// <param name="reviewDto">The review details entered by the user.</param>
        /// <returns>Redirects to the movie details page after adding the review.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int movieId, ReviewDto reviewDto)
        {
            reviewDto.MovieID = movieId;

            if (ModelState.IsValid)
            {
                var response = await _reviewService.AddReview(reviewDto);
                if (response.Status == ServiceResponse.ServiceStatus.Created)
                {
                    return RedirectToAction("Reviews", new { movieId = movieId });
                }
                else
                {
                    _logger.LogError($"Error adding review: {string.Join(", ", response.Messages)}");
                    ModelState.AddModelError("", string.Join(", ", response.Messages));
                }
            }

            return View("Reviews", new { movieId = movieId });  // Return the reviews view with validation errors.
        }

        /// <summary>
        /// Deletes a review for a movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie to delete the review from.</param>
        /// <param name="reviewId">The ID of the review to delete.</param>
        /// <returns>Redirects to the movie's reviews page after deletion.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int movieId, int reviewId)
        {
            var response = await _reviewService.DeleteReview(reviewId);
            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("Reviews", new { movieId = movieId });
            }
            else
            {
                _logger.LogError($"Error deleting review with ID {reviewId}: {string.Join(", ", response.Messages)}");
                return View("Error");
            }
        }
    }
}

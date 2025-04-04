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
            try
            {
                // Retrieve list of genres and origins
                ViewBag.Genres = await _movieService.GetGenresAsync();
                ViewBag.Origins = await _movieService.GetOriginsAsync();

                // Set default release date to today
                var movieDto = new MovieDto
                {
                    ReleaseDate = DateTime.Today
                };
                // Return the view with the initial model (MovieDto) to bind the form fields
                return View(new MovieDto());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading Create form: {ex.Message}");
                return View("Error");
            }
        }

        /// <summary>
        /// Handles the form submission to create a new movie.
        /// </summary>
        /// <param name="movieDto">The movie details entered by the user.</param>
        /// <returns>Redirects to the movie details page if successful, otherwise shows validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(MovieDto movieDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Log the incoming data for debugging
                    _logger.LogInformation($"Received movie submission: Title={movieDto.Title}, GenreCount={movieDto.GenreIds?.Count ?? 0}");

                    // Call the service to add the movie and genres
                    var response = await _movieService.AddMovie(movieDto);

                    if (response.Status == ServiceResponse.ServiceStatus.Created)
                    {
                        _logger.LogInformation($"Movie created successfully with ID: {response.CreatedId}");
                        // Redirect to the movie index page after successful creation
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // If there was an error, log it and return the error message to the view
                        _logger.LogWarning($"Movie creation failed: {string.Join(", ", response.Messages)}");
                        ModelState.AddModelError("", "An error occurred while creating the movie: " + string.Join(", ", response.Messages));
                    }
                }
                else
                {
                    // Log validation errors
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    _logger.LogWarning($"Model validation failed: {string.Join(", ", errors)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during movie creation: {ex.Message}");
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
            }

            // If the model is invalid or there was an error, return the form with current data
            // Load genres and origins for dropdown lists in the view again
            ViewBag.Genres = await _movieService.GetGenresAsync();
            ViewBag.Origins = await _movieService.GetOriginsAsync();
            return View(movieDto);
        }


        /// <summary>
        /// Displays the form for editing an existing movie.
        /// </summary>
        /// <param name="id">The ID of the movie to edit.</param>
        /// <returns>A view with the movie's existing details.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Fetch the movie details
                var movie = await _movieService.FindMovie(id);
                if (movie == null)
                {
                    _logger.LogWarning($"Movie with ID {id} not found.");
                    return NotFound();
                }

                // Fetch genres and origins for dropdowns
                ViewBag.Genres = await _movieService.GetGenresAsync();
                ViewBag.Origins = await _movieService.GetOriginsAsync();

                // Return the movie details for editing
                return View(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading Edit form for movie ID {id}: {ex.Message}");
                return View("Error");
            }
        }

        /// <summary>
        /// Handles the submission of edited movie details.
        /// </summary>
        /// <param name="id">The ID of the movie being edited.</param>
        /// <param name="movieDto">The updated movie details.</param>
        /// <returns>Redirects to the movie details page if successful, otherwise shows validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, MovieDto movieDto)
        {
            if (id != movieDto.MovieID)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"Updating movie ID {id}: Title={movieDto.Title}");

                    var response = await _movieService.UpdateMovie(id, movieDto);

                    if (response.Status == ServiceResponse.ServiceStatus.Updated)
                    {
                        _logger.LogInformation($"Movie ID {id} updated successfully.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning($"Movie update failed: {string.Join(", ", response.Messages)}");
                        ModelState.AddModelError("", "An error occurred while updating the movie: " + string.Join(", ", response.Messages));
                    }
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning($"Validation failed for movie ID {id}: {string.Join(", ", errors)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during movie update: {ex.Message}");
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
            }

            // Reload genres and origins in case of error
            ViewBag.Genres = await _movieService.GetGenresAsync();
            ViewBag.Origins = await _movieService.GetOriginsAsync();
            return View(movieDto);
        }

        /// <summary>
        /// Displays a confirmation page to delete a movie.
        /// </summary>
        /// <param name="id">The ID of the movie to delete.</param>
        /// <returns>A view displaying the movie information to confirm deletion.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var movie = await _movieService.FindMovie(id);
                if (movie == null)
                {
                    _logger.LogWarning($"Delete: Movie with ID {id} not found.");
                    return NotFound();
                }

                return View(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading Delete view: {ex.Message}");
                return View("Error");
            }
        }

        /// <summary>
        /// Handles the deletion of a movie after confirmation.
        /// </summary>
        /// <param name="id">The ID of the movie to delete.</param>
        /// <returns>Redirects to Index if successful, otherwise shows an error.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _movieService.DeleteMovie(id);

                if (response.Status == ServiceResponse.ServiceStatus.Deleted)
                {
                    _logger.LogInformation($"Movie with ID {id} deleted successfully.");
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning($"Movie delete failed for ID {id}: {string.Join(", ", response.Messages)}");
                ModelState.AddModelError("", "Failed to delete the movie.");
                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during movie deletion: {ex.Message}");
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

using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
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

        // GET: MoviePage
        // This action handles the request for displaying the list of movies on the movie page.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.ListMovies();
            if (movies == null)
            {
                _logger.LogWarning("No movies found.");
                return View("Error");  
            }
            return View(movies);  
        }

        // GET: MoviesPage/Details/5
        // This action handles requests to view the details of a specific movie.
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.FindMovie(id);
            if (movie == null)
            {
                _logger.LogWarning($"Movie with ID {id} not found.");
                return View("Error");  
            }
            return View(movie);  
        }

        // GET: MoviePage/Create
        // This action displays the form to create a new movie entry.
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Retrieve list of genres and origins
                ViewBag.Genres = await _movieService.GetGenresAsync();
                ViewBag.Origins = await _movieService.GetOriginsAsync();

                var movieDto = new MovieDto
                {
                    ReleaseDate = DateTime.Today
                };

                return View(new MovieDto());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading Create form: {ex.Message}");
                return View("Error");
            }
        }

        // POST: MoviePage/Create
        // This action handles the submission of a new movie entry and adds it to the database.
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

                    var response = await _movieService.AddMovie(movieDto);

                    if (response.Status == ServiceResponse.ServiceStatus.Created)
                    {
                        _logger.LogInformation($"Movie created successfully with ID: {response.CreatedId}");

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
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


        // GET: MoviePage/Edit/{id}
        // This action fetches the movie entry by ID and displays it in the edit form.
        [HttpGet]
        [Authorize]
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

        // POST: MoviePage/Edit/{id}
        // This action updates the existing movie entry with new data from the edit form.
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

        // GET: MoviePage/Delete/{id}
        // This action fetches a movie entry to confirm deletion.
        [HttpGet]
        [Authorize]
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

        // POST: MoviePage/Delete/{id}
        // This action deletes the movie entry from the database after confirmation.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
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

        // GET: MoviePage/Reviews/{movieId}
        // Displays the list of reviews for the specified movie.
        [HttpGet]
        public async Task<IActionResult> Reviews(int movieId)
        {
            var reviews = await _reviewService.ListReviewsByMovie(movieId);
            if (reviews == null)
            {
                _logger.LogWarning($"No reviews found for movie ID {movieId}.");
                return View("Error");
            }
            // Set the movieId in ViewData for easy access in the view
            ViewData["MovieId"] = movieId;

            return View(reviews);
        }

        // GET: MoviePage/AddReview
        // This action handles the request for displaying the review creation form for a movie.
        [HttpGet]
        [Authorize]
        public IActionResult AddReview(int movieId)
        {
            // Fetch movie details to include in the form
            var movie = _movieService.FindMovie(movieId);
            if (movie == null)
            {
                _logger.LogWarning($"Movie with ID {movieId} not found.");
                return View("Error");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reviewDto = new ReviewDto { 
                MovieID = movieId,
                UserID = userId
            };
            return View(reviewDto);
        }

        // POST: MoviePage/AddReview/{movieId}
        // Adds a new review to the specified movie after validating the input.
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReview(int movieId, ReviewDto reviewDto)
        {

            reviewDto.MovieID = movieId;
            reviewDto.ReviewDate = DateTime.Now;  // Ensure the date is set to current date and time

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

            return View(reviewDto); // Return the same view with validation errors
        }

        // GET: MoviesPage/DeleteReview/{movieId}/{reviewId}
        // Displays the confirmation page to delete a review.
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int movieId, int reviewId)
        {
            Console.WriteLine("here");
            // Fetch the review details using the reviewId
            var review = await _reviewService.FindReview(reviewId);

            if (review == null)
            {
                // If the review is not found, show an error page
                _logger.LogWarning($"Review with ID {reviewId} not found for movie ID {movieId}.");
                return View("Error");
            }

            // Pass the review details to the view so the user can confirm deletion
            ViewData["MovieId"] = movieId;
            return View(review);
        }



        // POST: MoviePage/DeleteReview
        // This action deletes a review for a movie.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ConfirmDeleteReview(int movieId, int reviewId)
        {
            var response = await _reviewService.DeleteReview(reviewId);
            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                ViewData["MovieId"] = movieId;
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

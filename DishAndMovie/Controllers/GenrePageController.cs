using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Mvc;

namespace DishAndMovie.Controllers
{
    public class GenrePageController : Controller
    {
        private readonly IGenreService _genreService;

        public GenrePageController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // GET: GenrePage
        // This action handles the request for displaying the list of genres on the genre page.
        // It retrieves all genres from the database and passes them to the view for rendering.
        public async Task<IActionResult> Index()
        {
            // Fetching the list of genres
            var genres = await _genreService.ListGenres();

            // Returning the genres to the view
            return View(genres);
        }

        // GET: GenrePage/Details/5
        // This action handles requests to view the details of a specific genre.
        // It fetches the genre by its ID and passes it to the view for rendering.
        public async Task<IActionResult> Details(int id)
        {
            // Retrieving genre details using the genre service
            var genre = await _genreService.FindGenre(id);

            // If the genre is not found, redirect to the index page
            if (genre == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Returning the genre details to the view
            return View(genre);
        }

        // GET: GenrePage/Create
        // This action serves the page for adding a new genre.
        // It returns an empty genre DTO to the view for user input.
        public IActionResult Create()
        {
            return View();
        }

        // POST: GenrePage/Create
        // This action handles the form submission for creating a new genre.
        // It takes the genre DTO, validates, and adds it to the database using the genre service.
        [HttpPost]
        public async Task<IActionResult> Create(GenreDto genreDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _genreService.AddGenre(genreDto);

                if (response.Status == ServiceResponse.ServiceStatus.Created)
                {
                    // Redirect to the genre list page after successful creation
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle failure if necessary, e.g. show an error message
                    ModelState.AddModelError("", "Failed to create genre.");
                }
            }

            // If model state is invalid, re-render the create view with validation errors
            return View(genreDto);
        }


        // GET: GenrePage/Edit/5
        // This action serves the page for editing an existing genre.
        // It fetches the genre by its ID and passes it to the view for editing.
        public async Task<IActionResult> Edit(int id)
        {
            var genre = await _genreService.FindGenre(id);
            if (genre == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(genre);
        }

        // POST: GenrePage/Edit/5
        // This action handles the form submission for updating an existing genre.
        // It takes the genre DTO, validates, and updates the genre in the database using the genre service.
        // POST: GenrePage/Edit/5
        // POST: GenrePage/Edit/{id}
        // POST: GenrePage/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GenreDto genreDto)
        {
            
            // Validate that the genre ID from the form matches the one in the URL
            if (id != genreDto.GenreID)
            {
                return BadRequest();
            }

            // If the model state is valid, attempt to update the genre
            if (ModelState.IsValid)
            {
                var result = await _genreService.UpdateGenre(genreDto);

                if (result.Status == ServiceResponse.ServiceStatus.Updated)
                {
                    return RedirectToAction("Index"); // Redirect to the index page if update is successful
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update the genre.");
                }
            }

            // Return to the Edit view with the updated model if validation fails
            return View(genreDto);
        }

        // GET: GenrePage/Delete/5
        // This action serves the page for confirming the deletion of a genre.
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _genreService.FindGenre(id);
            if (genre == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(genre);
        }

        // POST: GenrePage/Delete/5
        // This action handles the deletion of a genre from the database.
        // It deletes the genre and redirects to the index page.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _genreService.DeleteGenre(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

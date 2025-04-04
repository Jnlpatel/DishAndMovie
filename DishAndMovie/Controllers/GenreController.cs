using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Mvc;

namespace DishAndMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        /// <summary>
        /// Returns a list of all genres.
        /// </summary>
        /// <returns>A list of genre DTO objects.</returns>
        /// <example>
        /// GET /api/Genre/ListGenres
        /// Response:
        /// [
        ///   { "GenreID": 1, "Name": "Action" },
        ///   { "GenreID": 2, "Name": "Drama" }
        /// ]
        /// </example>
        [HttpGet("ListGenres")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> ListGenres()
        {
            var genres = await _genreService.ListGenres();
            return Ok(genres);
        }

        /// <summary>
        /// Returns the details of a specific genre.
        /// </summary>
        /// <param name="id">The ID of the genre to retrieve.</param>
        /// <returns>The genre DTO if found, otherwise a NotFound response.</returns>
        /// <example>
        /// GET /api/Genre/FindGenre/1
        /// Response:
        /// { "GenreID": 1, "Name": "Action" }
        /// </example>
        [HttpGet("FindGenre/{id}")]
        public async Task<ActionResult<GenreDto>> FindGenre(int id)
        {
            var genre = await _genreService.FindGenre(id);

            if (genre == null)
            {
                return NotFound(new { Message = "Genre not found." });
            }

            return Ok(genre);
        }

        /// <summary>
        /// Adds a new genre.
        /// </summary>
        /// <param name="genreDto">The genre data to add.</param>
        /// <returns>A response with the status of the creation operation.</returns>
        /// <example>
        /// POST /api/Genre/AddGenre
        /// Request Body:
        /// { "Name": "Comedy" }
        /// Response:
        /// { "Status": "Created", "Messages": ["Genre added successfully."] }
        /// </example>
        [HttpPost("AddGenre")]
        public async Task<ActionResult<ServiceResponse>> AddGenre([FromBody] GenreDto genreDto)
        {
            var response = await _genreService.AddGenre(genreDto);
            return StatusCode(201, response);
        }

        /// <summary>
        /// Updates an existing genre.
        /// </summary>
        /// <param name="genreDto">The updated genre data.</param>
        /// <returns>A response with the status of the update operation.</returns>
        /// <example>
        /// PUT /api/Genre/UpdateGenre
        /// Request Body:
        /// { "GenreID": 1, "Name": "Action/Adventure" }
        /// Response:
        /// { "Status": "Updated", "Messages": ["Genre updated successfully."] }
        /// </example>
        [HttpPut("UpdateGenre")]
        public async Task<ActionResult<ServiceResponse>> UpdateGenre([FromBody] GenreDto genreDto)
        {
            var response = await _genreService.UpdateGenre(genreDto);
            return Ok(response);
        }

        /// <summary>
        /// Deletes a genre by its ID.
        /// </summary>
        /// <param name="id">The ID of the genre to delete.</param>
        /// <returns>A response with the status of the deletion operation.</returns>
        /// <example>
        /// DELETE /api/Genre/DeleteGenre/1
        /// Response:
        /// { "Status": "Deleted", "Messages": ["Genre deleted successfully."] }
        /// </example>
        [HttpDelete("DeleteGenre/{id}")]
        public async Task<ActionResult<ServiceResponse>> DeleteGenre(int id)
        {
            var response = await _genreService.DeleteGenre(id);
            return Ok(response);
        }
    }
}

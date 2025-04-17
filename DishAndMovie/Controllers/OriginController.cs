using DishAndMovie.Models;
using Microsoft.AspNetCore.Mvc;
using DishAndMovie.Interfaces;
using DishAndMovie.Services;

namespace DishAndMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OriginController : ControllerBase
    {
        private readonly IOriginService _originService;
        private readonly IRecipeService _recipeService;
        private readonly IMovieService _movieService;

        // Dependency Injection for IOriginService
        public OriginController(IOriginService originService, IRecipeService recipeService, IMovieService movieService)
        {
            _originService = originService;
            _recipeService = recipeService;
            _movieService = movieService;

        }

        /// <summary>
        /// Returns a list of all origins.
        /// </summary>
        /// <returns>A list of origin DTO objects.</returns>
        /// <example>
        /// GET /api/Origin/ListOrigins
        /// Response:
        /// [
        ///   { "OriginId": 1, "OriginCountry": "Italy" },
        ///   { "OriginId": 2, "OriginCountry": "Japan" }
        /// ]
        /// </example>
        [HttpGet("ListOrigins")]
        public async Task<ActionResult<IEnumerable<OriginDto>>> ListOrigins()
        {
            IEnumerable<OriginDto> originDtos = await _originService.ListOrigins();
            return Ok(originDtos);
        }

        /// <summary>
        /// Returns a single origin by ID.
        /// </summary>
        /// <param name="id">The origin ID.</param>
        /// <returns>Origin DTO or 404 Not Found.</returns>
        /// <example>
        /// GET /api/Origin/FindOrigin/1
        /// Response:
        /// { "OriginId": 1, "OriginCountry": "Italy" }
        ///
        /// GET /api/Origin/FindOrigin/999
        /// Response: 404 Not Found
        /// </example>
        [HttpGet("FindOrigin/{id}")]
        public async Task<ActionResult<OriginDto>> FindOrigin(int id)
        {
            var origin = await _originService.FindOrigin(id);

            if (origin == null)
            {
                return NotFound();
            }
            return Ok(origin);
        }

        /// <summary>
        /// Updates an existing origin.
        /// </summary>
        /// <param name="id">The ID of the origin to update.</param>
        /// <param name="originDto">Updated origin details.</param>
        /// <returns>204 No Content or 404 Not Found.</returns>
        /// <example>
        /// PUT /api/Origin/UpdateOrigin/1
        /// Request body:
        /// { "OriginId": 1, "OriginCountry": "France" }
        /// Response: 204 No Content
        ///
        /// PUT /api/Origin/UpdateOrigin/999
        /// Request body:
        /// { "OriginId": 999, "OriginCountry": "Germany" }
        /// Response: 404 Not Found
        /// </example>
        [HttpPut("UpdateOrigin/{id}")]
        public async Task<IActionResult> UpdateOrigin(int id, OriginDto originDto)
        {
            if (id != originDto.OriginId)
            {
                return BadRequest("ID mismatch.");
            }

            ServiceResponse response = await _originService.UpdateOrigin(originDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return NoContent();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }

            return StatusCode(500, response.Messages);
        }

        /// <summary>
        /// Adds a new origin to the system.
        /// </summary>
        /// <param name="originDto">Origin details.</param>
        /// <returns>201 Created with Origin details.</returns>
        /// <example>
        /// POST /api/Origin/AddOrigin
        /// Request body:
        /// { "OriginId": 3, "OriginCountry": "India" }
        /// Response: 201 Created
        ///
        /// POST /api/Origin/AddOrigin
        /// Request body:
        /// { "OriginId": 4, "OriginCountry": "Mexico" }
        /// Response: 201 Created
        /// </example>
        [HttpPost("AddOrigin")]
        public async Task<ActionResult<OriginDto>> AddOrigin(OriginDto originDto)
        {
            ServiceResponse response = await _originService.AddOrigin(originDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return CreatedAtAction(nameof(FindOrigin), new { id = response.CreatedId }, originDto);
            }
            else
            {
                return StatusCode(500, response.Messages);
            }
        }

        /// <summary>
        /// Deletes an origin by ID.
        /// </summary>
        /// <param name="id">The ID of the origin to delete.</param>
        /// <returns>204 No Content or 404 Not Found.</returns>
        /// <example>
        /// DELETE /api/Origin/DeleteOrigin/1
        /// Response: 204 No Content
        ///
        /// DELETE /api/Origin/DeleteOrigin/999
        /// Response: 404 Not Found
        /// </example>
        [HttpDelete("DeleteOrigin/{id}")]
        public async Task<IActionResult> DeleteOrigin(int id)
        {
            ServiceResponse response = await _originService.DeleteOrigin(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }
        // GET api/origins/{originId}/movies
        [HttpGet("{originId}/movies")]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesByOrigin(int originId)
        {
            var movies = await _movieService.GetMoviesByOriginAsync(originId);
            var origin = await _originService.FindOrigin(originId);

            return Ok(new
            {
                Data = movies,
                Origin = origin?.OriginCountry ?? "Unknown Origin"
            });
        }

        // GET api/origins/{originId}/recipes
        [HttpGet("{originId}/recipes")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipesByOrigin(int originId)
        {
            var recipes = await _recipeService.GetRecipesByOriginAsync(originId);
            var origin = await _originService.FindOrigin(originId);

            return Ok(new
            {
                Data = recipes,
                Origin = origin?.OriginCountry ?? "Unknown Origin"
            });
        }
    }
}


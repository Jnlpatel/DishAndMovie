using DishAndMovie.Models;
using Microsoft.AspNetCore.Mvc;
using DishAndMovie.Interfaces;

namespace DishAndMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OriginController : ControllerBase
    {
        private readonly IOriginService _originService;

        // Dependency Injection for IOriginService
        public OriginController(IOriginService originService)
        {
            _originService = originService;
        }

        /// <summary>
        /// Returns a list of Origins. Pageable with optional parameters skip and perpage.
        /// </summary>
        /// <param name="skip">The number of records to skip, ordered by ID ascending.</param>
        /// <param name="perpage">The number of records to get.</param>
        /// <returns>
        /// 200 OK
        /// [{OriginDto}, {OriginDto}, ...]
        /// </returns>
        /// <example>
        /// GET: api/Origins/ListOrigins -> [{OriginDto}, {OriginDto}, ...]
        /// GET: api/Origins/ListOrigins?skip=0&perpage=10 -> [{OriginDto}, {OriginDto}, ... +8]
        /// </example>
        [HttpGet("ListOrigins")]
        public async Task<ActionResult<IEnumerable<OriginDto>>> ListOrigins(int? skip, int? perpage)
        {
            // Default to skip = 0 if not provided
            if (skip == null) skip = 0;

            // Default to perpage = 10 if not provided
            if (perpage == null) perpage = await _originService.CountOrigins();

            // Get paginated origins
            IEnumerable<OriginDto> originDtos = await _originService.ListOrigins((int)skip, (int)perpage);

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
    }
}

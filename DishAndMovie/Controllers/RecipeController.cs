using DishAndMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DishAndMovie.Interfaces;

namespace DishAndMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        /// <summary>
        /// Returns a list of Recipes. Pageable with optional parameters skip and perpage
        /// </summary>
        /// <param name="skip">The number of records to skip, ordered by ID ascending</param>
        /// <param name="perpage">The number of records to get</param>
        /// <returns>
        /// 200 OK
        /// [{RecipeDto},{RecipeDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Recipes/ListRecipes -> [{RecipeDto},{RecipeDto},..]
        /// GET: api/Recipes/ListRecipes?skip=0&perpage=10 -> [{RecipeDto},{RecipeDto},..+8]
        /// </example>
        [HttpGet("ListRecipes")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> ListRecipes(int? skip, int? perpage)
        {
            if (skip == null) skip = 0;
            if (perpage == null) perpage = await _recipeService.CountRecipes();

            IEnumerable<RecipeDto> recipeDtos = await _recipeService.ListRecipes((int)skip, (int)perpage);
            return Ok(recipeDtos);
        }


        /// <summary>
        /// Returns a single recipe by ID.
        /// </summary>
        /// <param name="id">The recipe ID.</param>
        /// <returns>Recipe DTO or 404 Not Found.</returns>
        /// <example>
        /// GET: api/Recipes/FindRecipe/1 ->
        /// {"RecipeId":1, "Name":"Spaghetti Bolognese", "Origin":"Italy"}
        /// </example>
        [HttpGet("FindRecipe/{id}")]
        public async Task<ActionResult<RecipeDto>> FindRecipe(int id)
        {
            var recipe = await _recipeService.FindRecipe(id);

            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        /// <summary>
        /// Updates an existing recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to update.</param>
        /// <param name="recipeDto">Updated recipe details.</param>
        /// <returns>204 No Content or 404 Not Found.</returns>
        /// <example>
        /// PUT: api/Recipes/UpdateRecipe/1
        /// Body: { "RecipeId": 1, "Name": "Spaghetti Carbonara", "Origin": "Italy" }
        /// </example>
        [HttpPut("UpdateRecipe/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateRecipe(int id, RecipeDto recipeDto)
        {
            if (id != recipeDto.RecipeId)
            {
                return BadRequest("ID in the URL does not match the Recipe ID in the body.");
            }

            ServiceResponse response = await _recipeService.UpdateRecipe(id, recipeDto);

            switch (response.Status)
            {
                case ServiceResponse.ServiceStatus.Updated:
                    return NoContent();
                case ServiceResponse.ServiceStatus.NotFound:
                    return NotFound(response.Messages);
                case ServiceResponse.ServiceStatus.Error:
                    return StatusCode(500, response.Messages);
                default:
                    return BadRequest(response.Messages);
            }
        }

        /// <summary>
        /// Adds a new recipe to the system.
        /// </summary>
        /// <param name="recipeDto">Recipe details.</param>
        /// <returns>201 Created with Recipe details.</returns>
        /// <example>
        /// POST: api/Recipes/AddRecipe ->
        /// { "RecipeId": 3, "Name": "Tacos", "Origin": "Mexico" }
        /// </example>
        [HttpPost("AddRecipe")]
        [Authorize]
        public async Task<ActionResult<RecipeDto>> AddRecipe(RecipeDto recipeDto)
        {
            ServiceResponse response = await _recipeService.AddRecipe(recipeDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Created($"api/Recipes/FindRecipe/{response.CreatedId}", recipeDto);
        }

        /// <summary>
        /// Deletes a recipe by ID.
        /// </summary>
        /// <param name="id">The ID of the recipe to delete.</param>
        /// <returns>204 No Content or 404 Not Found.</returns>
        /// <example>
        /// DELETE: api/Recipes/DeleteRecipe/1
        /// </example>
        [HttpDelete("DeleteRecipe/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            ServiceResponse response = await _recipeService.DeleteRecipe(id);

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

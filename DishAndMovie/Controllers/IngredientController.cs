using DishAndMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DishAndMovie.Interfaces;

namespace DishAndMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {

        private readonly IIngredientService _ingredientService;

        public IngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        /// <summary>
        /// Returns a list of all ingredients.
        /// </summary>
        /// <returns>A list of ingredient DTO objects.</returns>
        /// <example>
        /// GET: api/Ingredients/ListIngredients ->
        /// [
        ///  {"IngredientId":1, "Name":"Chicken Breast", "Unit":"grams", "CaloriesPerUnit":165},
        ///  {"IngredientId":2, "Name":"Avocado", "Unit":"grams", "CaloriesPerUnit":160}
        /// ]
        /// </example>
        [HttpGet(template: "ListIngredients")]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> ListIngredients()
        {
            // empty list of data transfer object IngredientDto
            IEnumerable<IngredientDto> ingredientDtos = await _ingredientService.ListIngredients();
            // return 200 OK with IngredientDto
            return Ok(ingredientDtos);
        }


        /// <summary>
        /// Returns a single ingredient by ID.
        /// </summary>
        /// <param name="id">The ingredient ID.</param>
        /// <returns>Ingredient DTO or 404 Not Found.</returns>
        /// <example>
        /// GET: api/Ingredients/FindIngredient/1 ->
        /// {"IngredientId":1, "Name":"Chicken Breast", "Unit":"grams", "CaloriesPerUnit":165}
        /// </example>
        [HttpGet(template: "FindIngredient/{id}")]
        public async Task<ActionResult<IngredientDto>> FindIngredient(int id)
        {
            var ingredient = await _ingredientService.FindIngredient(id);

            // if the ingredient could not be located, return 404 Not Found
            if (ingredient == null)
            {
                return NotFound();
            }
            return Ok(ingredient);
        }

        /// <summary>
        /// Updates an existing ingredient.
        /// </summary>
        /// <param name="id">The ID of the ingredient to update.</param>
        /// <param name="ingredientDto">Updated ingredient details.</param>
        /// <returns>204 No Content or 404 Not Found.</returns>
        /// <example>
        /// PUT: api/Ingredients/UpdateIngredient/1
        /// Body: { "IngredientId": 1, "Name": "Chicken Thigh", "Unit": "grams", "CaloriesPerUnit": 175 }
        /// </example>
        [HttpPut(template: "UpdateIngredient/{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateIngredient(int id, IngredientDto ingredientDto)
        {
            // Ensure the ID in the URL matches the ID in the request body
            if (id != ingredientDto.IngredientId)
            {
                return BadRequest("ID in the URL does not match the Ingredient ID in the body.");
            }

            // Call the service to update the ingredient
            ServiceResponse response = await _ingredientService.UpdateIngredient(ingredientDto);

            // Check the status of the response to determine the appropriate action
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
        /// Adds a new ingredient to the system.
        /// </summary>
        /// <param name="ingredientDto">Ingredient details.</param>
        /// <returns>201 Created with Ingredient details.</returns>
        /// <example>
        /// POST: api/Ingredients/AddIngredient ->
        /// { "IngredientId": 9, "Name": "Broccoli", "Unit": "grams", "CaloriesPerUnit": 55 }
        /// </example>
        [HttpPost(template: "AddIngredient")]
        //[Authorize]
        public async Task<ActionResult<IngredientDto>> AddIngredient(IngredientDto ingredientDto)
        {
            // Call the service to add the ingredient
            ServiceResponse response = await _ingredientService.AddIngredient(ingredientDto);

            // Check the status of the response to determine the appropriate action
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages); // Return 404 if the associated data was not found
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages); // Return 500 if there was an error
            }

            // Return 201 Created with the location of the new ingredient
            return Created($"api/Ingredients/FindIngredient/{response.CreatedId}", ingredientDto);
        }


        /// <summary>
        /// Deletes an ingredient by ID.
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete.</param>
        /// <returns>204 No Content or 404 Not Found.</returns>
        /// <example>
        /// DELETE: api/Ingredients/DeleteIngredient/1
        /// </example>
        [HttpDelete(template: "DeleteIngredient/{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            // Call the service to delete the ingredient by ID
            ServiceResponse response = await _ingredientService.DeleteIngredient(id);

            // Check the status of the response to determine the appropriate action
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(); // Return 404 if the ingredient was not found
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages); // Return 500 if there was an error
            }

            return NoContent(); // Return 204 No Content if the deletion was successful
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DishAndMovie.Data;
using DishAndMovie.Interfaces;
using DishAndMovie.Models;

namespace DishAndMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealPlansController : ControllerBase
    {

        private readonly IMealPlanService _mealPlanService;

        public MealPlansController(IMealPlanService mealPlanService)
        {
            _mealPlanService = mealPlanService;
        }


        /// <summary>
        /// Returns a list of Meal Plans
        /// </summary>
        /// <returns>A list of MealPlanDto objects</returns>
        /// <example>
        /// GET: api/MealPlans/ListMealPlans
        /// [
        ///   {"MealPlanId":1,"Name":"Weight Loss Plan","Date":"2025-02-01"},
        ///   {"MealPlanId":2,"Name":"High Protein Plan","Date":"2025-02-05"}
        /// ]
        /// </example>
        [HttpGet(template: "ListMealPlans")]
        public async Task<ActionResult<IEnumerable<MealPlanDto>>> ListMealPlans()
        {
            // Get the list of meal plans from the service
            IEnumerable<MealPlanDto> mealPlanDtos = await _mealPlanService.ListMealPlans();

            // Return 200 OK with the list of meal plans
            return Ok(mealPlanDtos);
        }


        /// <summary>
        /// Returns a single Meal Plan by ID
        /// </summary>
        /// <param name="id">The Meal Plan ID</param>
        /// <returns>200 OK with a MealPlanDto or 404 Not Found</returns>
        /// <example>
        /// GET: api/MealPlans/FindMealPlan/1
        /// {"MealPlanId":1,"Name":"Weight Loss Plan","Date":"2025-02-01"}
        /// </example>
        [HttpGet(template: "FindMealPlan/{id}")]
        public async Task<ActionResult<MealPlanDto>> FindMealPlan(int id)
        {
            var mealPlan = await _mealPlanService.FindMealPlan(id);

            // If the meal plan could not be located, return 404 Not Found
            if (mealPlan == null)
            {
                return NotFound();
            }

            return Ok(mealPlan);
        }


        /// <summary>
        /// Updates a Meal Plan
        /// </summary>
        /// <param name="id">The ID of the Meal Plan</param>
        /// <param name="mealPlanDto">Updated Meal Plan details</param>
        /// <returns>204 No Content or 400/404</returns>
        /// <example>
        /// PUT: api/MealPlans/UpdateMealPlan/1
        /// Body: { "MealPlanId": 1, "Name": "Diet Plan", "Date": "2025-02-02" }
        /// </example>
        [HttpPut(template: "UpdateMealPlan/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateMealPlan(int id, MealPlanDto mealPlanDto)
        {
            // Ensure the ID in the URL matches the ID in the request body
            if (id != mealPlanDto.MealPlanId)
            {
                return BadRequest("ID in the URL does not match the Meal Plan ID in the body.");
            }

            // Call the service to update the meal plan
            ServiceResponse response = await _mealPlanService.UpdateMealPlan(mealPlanDto);

            // Check the status of the response to determine the appropriate action
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages); // Return 404 if the meal plan was not found
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages); // Return 500 if there was an error
            }

            return NoContent(); // Return 204 No Content on successful update
        }


        /// <summary>
        /// Adds a new Meal Plan
        /// </summary>
        /// <param name="mealPlanDto">New Meal Plan details</param>
        /// <returns>201 Created or 400</returns>
        /// <example>
        /// POST: api/MealPlans/AddMealPlan
        /// Body: { "Name": "Diet Plan", "Date": "2025-02-20" }
        /// </example>
        [HttpPost(template: "AddMealPlan")]
        [Authorize]
        public async Task<ActionResult<MealPlan>> AddMealPlan(MealPlanDto mealPlanDto)
        {
            // Call the service to add the meal plan
            ServiceResponse response = await _mealPlanService.AddMealPlan(mealPlanDto);

            // Check the status of the response to determine the appropriate action
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages); // Return 404 if the associated data was not found
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages); // Return 500 if there was an error
            }

            // Return 201 Created with the location of the new meal plan
            return Created($"api/MealPlans/FindMealPlan/{response.CreatedId}", mealPlanDto);
        }

        /// <summary>
        /// Deletes a Meal Plan by ID
        /// </summary>
        /// <param name="id">The Meal Plan ID</param>
        /// <returns>204 No Content or 404 Not Found</returns>
        /// <example>
        /// DELETE: api/MealPlans/DeleteMealPlan/1
        /// </example>
        [HttpDelete(template: "DeleteMealPlan/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMealPlan(int id)
        {
            // Call the service to delete the meal plan by ID
            ServiceResponse response = await _mealPlanService.DeleteMealPlan(id);

            // Check the status of the response to determine the appropriate action
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(); // Return 404 if the meal plan was not found
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages); // Return 500 if there was an error
            }

            return NoContent(); // Return 204 No Content if the deletion was successful
        }
    }
}

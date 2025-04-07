using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DishAndMovie.Data.Migrations;
using DishAndMovie.Interfaces;
using DishAndMovie.Models;

namespace DishAndMovie.Controllers
{
    public class MealPlanPageController : Controller
    {
        private readonly IMealPlanService _mealPlanService;

        // Dependency injection of the meal plan service
        public MealPlanPageController(IMealPlanService mealPlanService)
        {
            _mealPlanService = mealPlanService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }


        // GET: MealPlanPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<MealPlanDto?> mealPlanDtos = await _mealPlanService.ListMealPlans();
            return View(mealPlanDtos);
        }


        // GET: MealPlanPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            MealPlanDto? mealPlanDto = await _mealPlanService.FindMealPlan(id); // Adjust this method call to match your service implementation

            if (mealPlanDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = ["Could not find mealplan"] });
            }

            return View(mealPlanDto);
        }


        // GET: /MealPlanPage/New
        [HttpGet]
        [Authorize]
        public IActionResult New()
        {
            return View();
        }

        // POST: MealPlanPage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(MealPlanDto mealPlanDto)
        {
            // Call your service to add the meal plan
            ServiceResponse response = await _mealPlanService.AddMealPlan(mealPlanDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                // Redirect to the List action to view the updated list
                return RedirectToAction("List");
            }
            else
            {
                // If there's an error, return the error view with messages
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: MealPlanPage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            MealPlanDto? mealPlanDto = await _mealPlanService.FindMealPlan(id);
            if (mealPlanDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find mealplan" } });
            }
            return View(mealPlanDto);
        }

        // POST: MealPlanPage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, MealPlanDto mealPlanDto)
        {
            if (!ModelState.IsValid)
            {
                return View(mealPlanDto); // Return the view with the current mealPlanDto to show validation errors
            }

            ServiceResponse response = await _mealPlanService.UpdateMealPlan(mealPlanDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }


        // GET MealPlanPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            MealPlanDto? mealPlanDto = await _mealPlanService.FindMealPlan(id);
            if (mealPlanDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(mealPlanDto);
            }
        }

        // POST MealPlanPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _mealPlanService.DeleteMealPlan(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List", "MealPlanPage");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

    }
}

using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishAndMovie.Controllers
{
    public class RecipePageController : Controller
    {
        private readonly IRecipeService _recipeService;

        // Dependency injection of the recipe service
        public RecipePageController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: RecipePage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<RecipeDto?> recipeDtos = await _recipeService.ListRecipes();
            return View(recipeDtos);
        }

        // GET: RecipePage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            RecipeDto? recipeDto = await _recipeService.FindRecipe(id);

            if (recipeDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find recipe" } });
            }

            return View(recipeDto);
        }

        // GET: RecipePage/New
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> New()
        {
            try
            {
                ViewBag.Origins = await _recipeService.GetOriginsAsync();
                return View(new RecipeDto());
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error loading New form: {ex.Message}");
                return View("Error", new ErrorViewModel { Errors = new List<string> { "Failed to load the form. " + ex.Message } });
            }
        }


        // POST: RecipePage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(RecipeDto recipeDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ServiceResponse response = await _recipeService.AddRecipe(recipeDto);

                    if (response.Status == ServiceResponse.ServiceStatus.Created)
                    {
                        return RedirectToAction("List");
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Join(", ", response.Messages));
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
            }

            ViewBag.Origins = await _recipeService.GetOriginsAsync();
            return View("New", recipeDto);
        }

        // GET: RecipePage/Edit/{id}
        // This action fetches the recipe entry by ID and displays it in the edit form.
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var recipeDto = await _recipeService.FindRecipe(id);
                if (recipeDto == null)
                {
                    return NotFound();
                }

                ViewBag.Origins = await _recipeService.GetOriginsAsync();
                return View(recipeDto);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new List<string> { $"An error occurred: {ex.Message}" }
                });
            }
        }

        // POST: RecipePage/Update/{id}
        // This action updates the existing recipe entry with new data from the edit form.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, RecipeDto recipeDto)
        {

            if (id != recipeDto.RecipeId)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _recipeService.UpdateRecipe(id, recipeDto);

                    if (response.Status == ServiceResponse.ServiceStatus.Updated)
                    {
                        return RedirectToAction(nameof(Details), new { id });
                    }

                    ModelState.AddModelError("", "An error occurred: " + string.Join(", ", response.Messages));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
            }

            ViewBag.Origins = await _recipeService.GetOriginsAsync();
            return View("Edit", recipeDto);
        }

        // GET: RecipePage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            RecipeDto? recipeDto = await _recipeService.FindRecipe(id);
            if (recipeDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find recipe" } });
            }
            return View(recipeDto);
        }

        // POST: RecipePage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _recipeService.DeleteRecipe(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}

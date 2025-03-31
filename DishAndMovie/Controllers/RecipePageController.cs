using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        //[Authorize]
        public IActionResult New()
        {
            return View(new RecipeDto());
        }

        // POST: RecipePage/Add
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Add(RecipeDto recipeDto)
        {
            if (!ModelState.IsValid)
            {
                return View("New", recipeDto);
            }

            ServiceResponse response = await _recipeService.AddRecipe(recipeDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: RecipePage/Edit/{id}
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            RecipeDto? recipeDto = await _recipeService.FindRecipe(id);
            if (recipeDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find recipe" } });
            }
            return View(recipeDto);
        }

        // POST: RecipePage/Update/{id}
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Update(int id, RecipeDto recipeDto)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", recipeDto);
            }

            ServiceResponse response = await _recipeService.UpdateRecipe(id, recipeDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: RecipePage/ConfirmDelete/{id}
        [HttpGet]
        //[Authorize]
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
        //[Authorize]
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

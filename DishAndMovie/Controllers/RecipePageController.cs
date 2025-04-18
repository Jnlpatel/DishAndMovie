using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using DishAndMovie.Models.ViewModels;
using DishAndMovie.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishAndMovie.Controllers
{
    public class RecipePageController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;

        // Dependency injection of the recipe service
        public RecipePageController(IRecipeService recipeService, IIngredientService ingredientService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: RecipePage/List?PageNum={pagenum}
        // GET: RecipePage/List
        [HttpGet]
        public async Task<IActionResult> List(int PageNum = 0)
        {
            int PerPage = 3;

            // Get total recipe count from service
            int totalCount = await _recipeService.CountRecipes();

            int MaxPage = (int)Math.Ceiling((decimal)totalCount / PerPage) - 1;

            // Ensure boundaries are respected
            if (MaxPage < 0) MaxPage = 0;
            if (PageNum < 0) PageNum = 0;
            if (PageNum > MaxPage) PageNum = MaxPage;

            int StartIndex = PageNum * PerPage;

            // Fetch only paginated recipes
            IEnumerable<RecipeDto?> recipeDtos = await _recipeService.ListRecipes(StartIndex, PerPage); // Make sure this overload exists

            RecipeList viewModel = new RecipeList
            {
                Recipes = recipeDtos,
                Page = PageNum,
                MaxPage = MaxPage
            };

            return View(viewModel);
        }


        // GET: RecipePage/Details/{id}
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            RecipeDto? recipeDto = await _recipeService.FindRecipe(id);

            if (recipeDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find recipe" } });
            }

            // Ingredients used in the recipe
            var ingredients = await _recipeService.GetIngredientsForRecipeAsync(id);
            recipeDto.IngredientsUsed = ingredients.ToList();

            // All available ingredients for dropdown
            var allIngredients = await _ingredientService.GetAllIngredientsAsync();
            ViewBag.AllIngredients = allIngredients;

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

        // POST: RecipePage/AddIngredient/{recipeId}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddIngredient(int recipeId, int ingredientId, decimal quantity)
        {
            var response = await _recipeService.AddIngredientToRecipeAsync(recipeId, ingredientId, quantity);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction(nameof(Details), new { id = recipeId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // POST: RecipePage/RemoveIngredient
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveIngredient(int recipeId, int ingredientId)
        {
            await _recipeService.RemoveIngredientFromRecipeAsync(recipeId, ingredientId);
            return RedirectToAction(nameof(Details), new { id = recipeId });
        }


    }
}

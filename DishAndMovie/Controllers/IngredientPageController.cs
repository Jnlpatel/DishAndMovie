using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DishAndMovie.Controllers
{
    public class IngredientPageController : Controller
    {
        private readonly IIngredientService _ingredientService;

        // Dependency injection of the ingredient service
        public IngredientPageController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: IngredientPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<IngredientDto?> ingredientDtos = await _ingredientService.ListIngredients();
            return View(ingredientDtos);
        }

        // GET: IngredientPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            IngredientDto? ingredientDto = await _ingredientService.FindIngredient(id);

            if (ingredientDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find ingredient" } });
            }

            return View(ingredientDto);
        }

        // GET: IngredientPage/New
        [HttpGet]
        //[Authorize]
        public IActionResult New()
        {
            return View(new IngredientDto());
        }

        // POST: IngredientPage/Add
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Add(IngredientDto ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View("New", ingredientDto);
            }

            ServiceResponse response = await _ingredientService.AddIngredient(ingredientDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: IngredientPage/Edit/{id}
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            IngredientDto? ingredientDto = await _ingredientService.FindIngredient(id);
            if (ingredientDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find ingredient" } });
            }
            return View(ingredientDto);
        }

        // POST: IngredientPage/Update/{id}
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Update(int id, IngredientDto ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", ingredientDto);
            }

            ServiceResponse response = await _ingredientService.UpdateIngredient(ingredientDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: IngredientPage/ConfirmDelete/{id}
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            IngredientDto? ingredientDto = await _ingredientService.FindIngredient(id);
            if (ingredientDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Could not find ingredient" } });
            }
            return View(ingredientDto);
        }

        // POST: IngredientPage/Delete/{id}
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _ingredientService.DeleteIngredient(id);

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

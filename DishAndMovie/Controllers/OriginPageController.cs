using DishAndMovie.Interfaces;
using DishAndMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DishAndMovie.Controllers
{
    public class OriginPageController : Controller
    {
        private readonly IOriginService _originService;

        // Dependency injection for IOriginService
        public OriginPageController(IOriginService originService)
        {
            _originService = originService;
        }

        // Redirecting to List action when the Index is hit
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: OriginPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<OriginDto?> originDtos = await _originService.ListOrigins();
            return View(originDtos); // Returning list of origins to view
        }

        // GET: OriginPage/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            OriginDto? originDto = await _originService.FindOrigin(id);
            if (originDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Origin not found" } });
            }
            return View(originDto); // Showing details of a single origin
        }

        // GET: OriginPage/New
        [HttpGet]
        [Authorize]
        public IActionResult New()
        {
            return View(new OriginDto()); // New origin creation view
        }

        // POST: OriginPage/Add
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(OriginDto originDto)
        {
            if (!ModelState.IsValid)
            {
                return View("New", originDto); // Returning back if model is not valid
            }

            ServiceResponse response = await _originService.AddOrigin(originDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("List"); // Redirecting to list on successful creation
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: OriginPage/Edit/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            OriginDto? originDto = await _originService.FindOrigin(id);
            if (originDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Origin not found" } });
            }
            return View(originDto); // Edit view for existing origin
        }

        // POST: OriginPage/Update/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(int id, OriginDto originDto)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", originDto); // Returning back if model is not valid
            }

            ServiceResponse response = await _originService.UpdateOrigin(originDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: OriginPage/ConfirmDelete/{id}
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            OriginDto? originDto = await _originService.FindOrigin(id);
            if (originDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new List<string> { "Origin not found" } });
            }
            return View(originDto); // Confirm delete page
        }

        // POST: OriginPage/Delete/{id}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _originService.DeleteOrigin(id);

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

using DishAndMovie.Services;
using Microsoft.AspNetCore.Mvc;

namespace DishAndMovie.Controllers
{
 
    public class WeatherController : Controller
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<IActionResult> Index(string city = "Toronto")
        {
            var weatherJson = await _weatherService.GetWeatherAsync(city);

            if (weatherJson == null)
            {
                ViewBag.Error = "Could not fetch weather data";
                return View();
            }

            ViewBag.City = city;
            return View(model: weatherJson);
        }
    }

}

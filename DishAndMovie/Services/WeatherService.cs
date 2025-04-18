using System.Net.Http;
using System.Threading.Tasks;
namespace DishAndMovie.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetWeatherAsync(string city)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://goweather.herokuapp.com/weather/{city}");
                response.EnsureSuccessStatusCode(); // Throws if HTTP error
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                return null; // Or return a default JSON string
            }
        }
    }
}

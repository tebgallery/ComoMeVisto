using ComoMeVisto.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace ComoMeVisto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherInfoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherInfoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet(Name = "GetWeatherInfo")]
        public async Task<IActionResult> GetWeather(string city)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // Configurar la URL de la API de pronóstico del clima
                string apiKey = "ef8fcd8d92838aa93d1ed34adcc2375b"; // Reemplaza con tu clave de API real
                string apiUrl = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<WeatherInfo>(content);

                    return Ok(weatherData);
                }

                return StatusCode((int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

﻿using ComoMeVisto.Models;
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
                string apiKey = "ef8fcd8d92838aa93d1ed34adcc2375b";
                string apiUrl = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<Forecast>(content);

                    var weatherForecasts = ProcessWeatherData(weatherData);

                    return Ok(weatherForecasts);
                }

                return StatusCode((int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private List<WeatherForecast> ProcessWeatherData(Forecast forecast)
        {
            var weatherForecasts = new List<WeatherForecast>();
            Clothing clothing = new Clothing();

            foreach (var weatherList in forecast.list)
            {
                // Calcula la recomendación de vestimenta basada en las condiciones climáticas
                //string clothingRecommendation = DetermineClothingRecommendation(forecast);

                // Crea una instancia de WeatherForecast y agrega los datos

                foreach (var weathers in weatherList.weather)
                {
                    var weatherForecast = new WeatherForecast
                    {
                        CityName = forecast.city.name,
                        Temperature = weatherList.main.temp,
                        Humidity = Convert.ToString(weatherList.main.humidity) + "%",
                        Weather = weathers.main,
                        Description = weathers.description,
                        WindSpeed = weatherList.wind.speed,
                        Time = (Convert.ToDateTime(weatherList.dt_txt).AddHours(-3)).ToShortTimeString(),
                        Date = Convert.ToDateTime(weatherList.dt_txt).ToShortDateString(),
                        Clothing = DetermineClothing(weatherList.main.temp, weatherList.main.humidity, weathers.main, weathers.description, weatherList.wind.speed),
                    };
                    weatherForecasts.Add(weatherForecast);
                }
            }

            return weatherForecasts;
        }

        private Clothing DetermineClothing(double temp,double hum,string weather,string weatherDesc,double windSpeed)
        {
            Clothing clothing = new Clothing();

            if (temp < 13)
            {
                clothing.Cap = true;
                clothing.WoolenCap = true;
                clothing.Shirt = true;
                clothing.ThickJacket = true;
                clothing.LongPants = true;
            }
            else if (temp < 20)
            {
                clothing.Cap = true;
                clothing.Shirt = true;
                clothing.LightJacket = true;
                clothing.LongPants = true;
            }
            else if (temp >= 20)
            {
                clothing.Cap = true;
                clothing.Shirt = true;
                clothing.Shorts = true;
            }

            return clothing;

        }

    }
}

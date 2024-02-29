using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyWeatherApi.Services;

namespace MyWeatherApi.Controllers;

public class WeatherApiController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly WeatherApiService _weatherApiService;

    public WeatherApiController(IConfiguration configuration, WeatherApiService weatherApiService)
    {
        _configuration = configuration;
        _weatherApiService = weatherApiService;
    }

    public IActionResult GetWeatherData(string city)
    {
        if (string.IsNullOrEmpty(city))
            return BadRequest();

        var authHeader = HttpContext.Request.Headers["apiKey"];
        if (string.IsNullOrEmpty(authHeader) || authHeader != _configuration.GetValue<string>("Api:UserApiKey"))
            return Unauthorized();
        
        var data = _weatherApiService.GetWeatherData(city);

        return Json(data);
    }
}
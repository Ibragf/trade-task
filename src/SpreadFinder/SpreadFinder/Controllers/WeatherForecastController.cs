using Microsoft.AspNetCore.Mvc;

namespace SpreadFinder.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]
    public string Get()
    {
        return "Ну сегодня шикарная погода";
    }
}
using Microsoft.AspNetCore.Mvc;

namespace WorkshopDemo.WeatherForecast;

[ApiController]
[Route("api/WeatherForecast")]
public class WeatherForecastController
{
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        var forecasts =  Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecasts;
    }
    
}
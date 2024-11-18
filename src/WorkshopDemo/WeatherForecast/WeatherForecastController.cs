using Microsoft.AspNetCore.Mvc;

namespace WorkshopDemo.WeatherForecast;

[ApiController]
[Route("api/WeatherForecast")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger)
{
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        logger.LogInformation("Get WeatherForecast called");
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

 
    [HttpGet("slow")]
    public async Task<IEnumerable<WeatherForecast>> GetSlow()
    {
        var randomWaitInMs = Random.Shared.Next(500, 5000);
        await Task.Delay(randomWaitInMs);

        logger.LogInformation("Get Slow WeatherForecast called with wait of {wait}", randomWaitInMs);
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
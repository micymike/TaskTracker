using Newtonsoft.Json;
using System.Net.Http;

namespace TaskTracker.Services;

public class WeatherService
{
    private const string ApiKey = "47b3e71148ce48442336d18d325e3bda"; // 
    private const string Url = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric";

    public async Task<string> GetWeatherAsync(string city)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync(string.Format(Url, city, ApiKey));
            dynamic? data = JsonConvert.DeserializeObject(response);
            if (data == null) return "Weather data unavailable";
            return $"{data.main.temp}°C, {data.weather[0].description}";
        }
        catch (Exception ex)
        {
            return $"Weather unavailable: {ex.Message}";
        }
    }
}
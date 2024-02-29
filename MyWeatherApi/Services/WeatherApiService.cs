using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace MyWeatherApi.Services;

public class WeatherApiService
{
    private readonly WeatherDataStorageService _storageService;
    private readonly IConfiguration _configuration;
    private readonly string? _apiKey;

    public WeatherApiService(WeatherDataStorageService storageService, IConfiguration configuration)
    {
        _storageService = storageService;
        _configuration = configuration;
        _apiKey = _configuration.GetValue<string>("Api:WeatherApiKey");
    }

    private Dictionary<string, string> GetGeocodingAboutCityData(string city)
    {
        
        string url =
            $"https://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_apiKey}";
        try
        {
            using WebClient client = new WebClient();
            string data = client.DownloadString(url);
            JsonDocument document = JsonDocument.Parse(data);
            var element = document.RootElement;
            var output = new Dictionary<string, string>()
            {
                { "lat", element[0].GetProperty("lat").ToString() },
                { "lon", element[0].GetProperty("lon").ToString() }
            };
            return output;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return null;
        }
    }

    public JsonDocument GetWeatherData(string city)
    {
        var cache = _storageService.LoadCache(city);
        if (cache != null)
            return cache;
        
        var coords = GetGeocodingAboutCityData(city);
        
        string url =
            $"https://api.openweathermap.org/data/2.5/weather?lat={coords["lat"]}&lon={coords["lon"]}&appid=35b07252441dd88ece12ece17f51cb0e&units=metric";
        
        try
        {
            using WebClient client = new WebClient();
            var data = client.DownloadString(url);
            
            var doc =  JsonDocument.Parse(data);
            _storageService.UpdateCache(doc, city);
            return doc;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return null;
        }
    }
}
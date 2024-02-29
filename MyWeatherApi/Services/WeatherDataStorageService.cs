using System.Text.Json;

namespace MyWeatherApi.Services;

public class WeatherDataStorageService
{
    private const string CACHE_PATH = "./weatherCache";
    private const string CACHE_EXT = ".json";
    private readonly int MAX_HOUR_DELAY = 5;

    private readonly IConfiguration _configuration;

    public WeatherDataStorageService(IConfiguration configuration)
    {
        _configuration = configuration;
        MAX_HOUR_DELAY = _configuration.GetValue<int>("ExpirationHours");
    }

    public JsonDocument? LoadCache(string city)
    {
        string path = GetCacheFilePath(city);
        if (!File.Exists(path))
            return null;

        if (GetDelayInHours(File.GetLastWriteTimeUtc(path)) > MAX_HOUR_DELAY)
            return null;

        var data = File.ReadAllText(path);
        return JsonDocument.Parse(data);
    }

    public void UpdateCache(JsonDocument data, string city)
    {
        string path = GetCacheFilePath(city);
        if (!Directory.Exists(CACHE_PATH))
            Directory.CreateDirectory(CACHE_PATH);
        File.WriteAllText(path, data.RootElement.GetRawText());
    }

    private string GetCacheFilePath(string city)
    {
        return $"{CACHE_PATH}/{city.ToLower()}{CACHE_EXT}";
    }

    private int GetDelayInHours(DateTime time)
    {
        var delay = new TimeSpan(DateTime.UtcNow.Ticks - time.Ticks);
        return delay.Hours;
    }
}
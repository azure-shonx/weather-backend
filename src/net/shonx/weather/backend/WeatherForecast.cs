namespace net.shonx.weather.backend;

using Newtonsoft.Json;

public class WeatherForecast(int zipcode, int temperature, string? summary, bool isRainy)
{
    public int Zipcode { get; } = zipcode;
    public int Temperature { get; } = temperature;

    public string? Summary { get; } = summary;

    public bool IsRaining { get; } = isRainy;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
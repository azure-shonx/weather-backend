using Newtonsoft.Json;

public class WeatherForecast
{

    public WeatherForecast(int zipcode, int temperature, string? summary, bool isRainy)
    {
        this.zipcode = zipcode;
        this.temperature = temperature;
        this.summary = summary;
        this.isRainy = isRainy;
    }
    public int zipcode { get; }
    public int temperature { get; }

    public string? summary { get; }

    public bool isRainy { get; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
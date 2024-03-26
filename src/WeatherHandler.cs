public class WeatherHandler
{


    public WeatherHandler()
    {

    }

    public WeatherForecast GetForecast(int zipcode)
    {
        bool isRainy = (Random.Shared.Next() >= (Int32.MaxValue / 2));
        int temperature = Random.Shared.Next(-20, 55);
        string summary = GetSummary(temperature, isRainy);
        return new WeatherForecast(zipcode, temperature, summary, isRainy);
    }


    private string GetSummary(int temperature, bool isRainy)
    {
        if (isRainy && temperature < 0)
            return "Snowing";
        if (isRainy)
            return "Raining";
        if (temperature <= 0)
            return "Freezing";
        if (temperature > 0 && temperature <= 10)
            return "Cold";
        if (temperature > 10 && temperature <= 20)
            return "Chilly";
        if (temperature > 20 && temperature <= 30)
            return "Mild";
        if (temperature > 30 && temperature <= 40)
            return "Hot";
        if (temperature > 40)
            return "Scorching";
        throw new InvalidOperationException("Invalid summary");
    }
}
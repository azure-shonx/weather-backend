namespace net.shonx.weather.backend.web;

using net.shonx.weather.backend.handlers;
using Newtonsoft.Json;
using System.Net;


public class RequestProcessor
{
    private readonly AzureStorageHandler AzureStorageHandler;
    private readonly WeatherHandler WeatherHandler;

    public RequestProcessor(AzureStorageHandler AzureStorageHandler, WeatherHandler WeatherHandler)
    {
        this.AzureStorageHandler = AzureStorageHandler;
        this.WeatherHandler = WeatherHandler;
    }


    public async Task TestRoot(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        await WebUtil.WriteLines(context.Response, "{\"status\": \"OK\"}");
    }

    public void InvalidGetWeatherForecast(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }

    public async Task GetWeatherForecast(HttpContext context, int zipcode)
    {
        await WebUtil.WriteLines(context.Response, JsonConvert.SerializeObject(WeatherHandler.GetForecast(zipcode)));
    }

    public async Task RegisterEmail(HttpContext context)
    {
        Email? email = await WebUtil.GetRequest<Email>(context.Request);
        if (email is null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }
        if (email.Zipcode <= 0 || email.Zipcode >= 10000)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }
        AzureStorageHandler.AddEmail(email);
        context.Response.StatusCode = (int)HttpStatusCode.OK;
    }

    public async Task DeleteEmail(HttpContext context)
    {
        Email? email = await WebUtil.GetRequest<Email>(context.Request);
        if (email is null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }
        AzureStorageHandler.DeleteEmail(email);
        context.Response.StatusCode = (int)HttpStatusCode.OK;
    }

    public async Task GetEmails(HttpContext context)
    {
        await WebUtil.WriteLines(context.Response, JsonConvert.SerializeObject(AzureStorageHandler.GetEmails()));
    }
}
namespace net.shonx.weather.backend.web;

public class WebHandler
{
    private readonly WebApplicationBuilder builder;
    private readonly WebApplication app;
    private readonly RequestProcessor RequestProcessor;

    public WebHandler(string[] args)
    {
        builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        RequestProcessor = new(new(), new());

        BuildMappings();
    }

    public void Run()
    {
        app.Run();
    }

    private void BuildMappings()
    {
        app.MapGet("/", RequestProcessor.TestRoot).WithName("TestRoot");

        app.MapGet("/weather/get/", RequestProcessor.InvalidGetWeatherForecast).WithName("InvalidGetWeatherForecast");

        app.MapGet("/weather/get/{zipcode}", RequestProcessor.GetWeatherForecast).WithName("GetWeatherForecast");

        app.MapPut("/emails/add/", RequestProcessor.RegisterEmail).WithName("RegisterEmail");

        app.MapPut("/emails/remove/", RequestProcessor.DeleteEmail).WithName("DeleteEmail");

        app.MapGet("/emails/get", RequestProcessor.GetEmails).WithName("GetEmails");
    }
}
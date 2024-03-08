using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        AzureHandler handler = new AzureHandler();

        app.UseHttpsRedirection();
        
        app.MapGet("/", () => {
            return "CONNECTION SUCCESSFUL";
        })
        .WithName("TestRoot")
        .WithOpenApi();


        app.MapPut("/emails/add/", async context =>
        {
			Console.WriteLine("GOT REQUEST.");
            if (context.Request.ContentType != "application/json")
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            string json = await ReadLines(context.Request.Body);
            Email email;
            try
            {
                email = JsonConvert.DeserializeObject<Email>(json);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            if (email is null || email.email is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            handler.AddEmail(email);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
        })
        .WithName("RegisterEmail")
        .WithOpenApi();

        app.Run();
    }

    private static async Task<string> ReadLines(Stream stream)
    {
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            string result = "";
            string line;
            while (true)
            {
                line = await reader.ReadLineAsync();
                if (line is null)
                    return result;
                result += line;
            }
        }
    }
}


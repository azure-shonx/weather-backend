using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

public class WebHandler
{

    private WebApplicationBuilder builder;
    private WebApplication app;
    private AzureHandler handler;
    public WebHandler(string[] args)
    {
        buildWebApp(args);
    }

    private void buildWebApp(string[] args)
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

        handler = new AzureHandler();

        app.UseHttpsRedirection();

        app.MapGet("/", () =>
        {
            return "CONNECTION SUCCESSFUL";
        })
        .WithName("TestRoot")
        .WithOpenApi();


        app.MapPut("/emails/add/", async context =>
        {
            Email email = await GetEmailAndSetResponse(context.Request, context.Response);
            if (email is null)
            {
                Console.WriteLine("email object is null.");
                return;
            }
            if (email.zipcode <= 0)
            {
                Console.WriteLine("Provided zipcode is 0 (or less).");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            handler.AddEmail(email);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
        })
        .WithName("RegisterEmail")
        .WithOpenApi();

        app.MapPut("/emails/remove/", async context =>
        {
            Email email = await GetEmailAndSetResponse(context.Request, context.Response);
            if (email is null)
                return;
            handler.DeleteEmail(email);
            context.Response.StatusCode = (int)HttpStatusCode.OK;

        })
        .WithName("DeleteEmail")
        .WithOpenApi();

        app.MapGet("/emails/get", () =>
        {
            return JsonConvert.SerializeObject(handler.GetEmails());
        })
        .WithName("GetEmails")
        .WithOpenApi();

        app.Run();
    }

    private async Task<Email> GetEmailAndSetResponse(HttpRequest request, HttpResponse response)
    {
        Email email;
        if (request.ContentType.Contains("application/json"))
        {
            Console.WriteLine("Invalid ContentType. ContentType is \"" + request.ContentType + "\"");
            Console.WriteLine("No application/json in Header.");
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }
        string json = await ReadLines(request.Body);
        try
        {
            email = JsonConvert.DeserializeObject<Email>(json);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception decoding Json.");
            Console.WriteLine(e.ToString());
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }
        if (email is null || String.IsNullOrEmpty(email.email))
        {
            Console.WriteLine("Bad email.");
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }
        return email;
    }

    private async Task<string> ReadLines(Stream stream)
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
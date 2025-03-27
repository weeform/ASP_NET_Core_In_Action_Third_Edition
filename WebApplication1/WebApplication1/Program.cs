using Microsoft.AspNetCore.HttpLogging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Mime;
using System.Reflection;

ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

// Create a WebAPplicationBuilder instance
var builder = WebApplication.CreateBuilder(args);

// Register the required services and configuration with the WebApplicationBuilder
builder.Services.AddHttpLogging(opts => opts.LoggingFields = HttpLoggingFields.RequestProperties);
builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);
builder.Services.AddProblemDetails();

// Call Build() on the builder instance to create a WebApplication instance
var app = builder.Build();

// Add middleware to the WebApplication to create a pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();
app.UseWelcomePage("/welcome");
app.UseStaticFiles();
app.UseRouting();

// Map the endpoints to the WebApplication
// You can define the endpoints for your app by using MapGet() anywhere in Program.cs before the call to app.Run(),
// but the calls are typically placed after the middleware pipeline definition
app.MapGet("/", () => Results.Redirect("/welcome"));
app.MapGet("/hello", () => "Hello World!");

var people = new List<Person> {
    new("Huifeng", "Li"),
    new("John", "Doe"),
    new("A1", "Pacino")
};
app.MapGet("/person/{name}", (string name) => people.Where(p => p.FirstName.StartsWith(name)));
app.MapGet("/error", () => "Sorry, an error occurred");
app.MapGet("/throw", (HttpContext context) => throw new Exception("This is a test exception"));
app.MapFallback(() => Results.Redirect("/welcome"));

app.MapGet("/teapot", (HttpResponse response) => {
    response.StatusCode = 418;
    response.ContentType = MediaTypeNames.Text.Plain;
    return response.WriteAsync("I'm a teapot!");
});

var _fruit = new ConcurrentDictionary<string, Fruit>();
app.MapGet("/fruit", () => _fruit);
app.MapGet("/fruit/{id}", (string id) => _fruit.TryGetValue(id, out var fruit)
        ? Results.Ok(fruit)
        : Results.Problem(statusCode: 404))
    .AddEndpointFilter<IdValidationFilter>();

app.MapPost("/fruit/{id}", (string id, Fruit fruit) => _fruit.TryAdd(id, fruit)
    ? TypedResults.Created($"/fruit/{id}", fruit)
    : Results.ValidationProblem(new Dictionary<string, string[]> {
        { "id", new [] {"A fruit with this id already exists"} }
    }))
    .AddEndpointFilterFactory(ValidationHelper.ValidateIdFactory); ;

app.MapPut("/fruit/{id}", (string id, Fruit fruit) => {
    _fruit[id] = fruit;
    return Results.NoContent();
});

app.MapDelete("/fruit/{id}", (string id) =>
{
    _fruit.TryRemove(id, out _);
    return Results.NoContent();
});

// Call Run() on the WebApplication to start the server and handle requests
app.Run();

record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
};

public record Person(string FirstName, string LastName);

class ValidationHelper
{
    internal static EndpointFilterDelegate ValidateIdFactory(
        EndpointFilterFactoryContext context,
        EndpointFilterDelegate next)
    {
        ParameterInfo[] parameters = context.MethodInfo.GetParameters();
        int? idPostion = null;
        for (int i = 0; i < parameters.Length; i++) {
            if (parameters[i].Name == "id" && parameters[i].ParameterType == typeof(string))
            {
                idPostion = i;
                break;
            }
        }
        if (!idPostion.HasValue)
        {
            return next;
        }
        return async (invocationContext) =>
        {
            var id = invocationContext.GetArgument<string>(idPostion.Value);
            if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]> {
                        {"id", new[] { "Invalid format. Id must start with 'f'"} }
                    });
            }
            return await next(invocationContext);
        };
    }


    internal static async ValueTask<object?> ValidateId(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ValidationHelper>>();
        logger.LogInformation("Executing ValidateId method");

        var id = context.GetArgument<string>(0);
        if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]> {
                    {"id", new[] { "Invalid format. Id must start with 'f'"} }
                });
        }
        return await next(context);
    }
}

class IdValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var id = context.GetArgument<string>(0);
        if (string.IsNullOrEmpty(id) || !id.StartsWith('f'))
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]> {
                    {"id", new[]{ "Invalid format. Id must start with 'f'"} }
                });
        }
        return await next(context);
    }
}
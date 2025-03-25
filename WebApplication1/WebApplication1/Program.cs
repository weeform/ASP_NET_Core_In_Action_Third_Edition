using Microsoft.AspNetCore.HttpLogging;
using System.Collections.Concurrent;
using System.Net;

ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

// Create a WebAPplicationBuilder instance
var builder = WebApplication.CreateBuilder(args);

// Register the required services and configuration with the WebApplicationBuilder
builder.Services.AddHttpLogging(opts => opts.LoggingFields = HttpLoggingFields.RequestProperties);
builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

// Call Build() on the builder instance to create a WebApplication instance
var app = builder.Build();

// Add middleware to the WebApplication to create a pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseWelcomePage("/welcome");
app.UseStaticFiles();
app.UseRouting();

// Map the endpoints to the WebApplication
// You can define the endpoints for your app by using MapGet() anywhere in Program.cs before the call to app.Run(),
// but the calls are typically placed after the middleware pipeline definition
app.MapGet("/hello", () => "Hello World!");

var people = new List<Person> {
    new("Huifeng", "Li"),
    new("John", "Doe"),
    new("A1", "Pacino")
};
app.MapGet("/person/{name}", (string name) => people.Where(p => p.FirstName.StartsWith(name)));
app.MapGet("/error", (HttpContext context) => throw new Exception("exception when generate an exception page"));
app.MapGet("/throw", (HttpContext context) => throw new Exception("This is a test exception"));
app.MapFallback(() => Results.Redirect("/welcome"));

var _fruit = new ConcurrentDictionary<string, Fruit>();
app.MapGet("/fruit", () => _fruit);
app.MapGet("/fruit/{id}", (string id) => _fruit.TryGetValue(id, out var fruit) ? Results.Ok(fruit) : Results.NotFound());
app.MapPost("/fruit/{id}", (string id, Fruit fruit) => _fruit.TryAdd(id, fruit)
    ? TypedResults.Created($"/fruit/{id}", fruit)
    : Results.BadRequest(new { id = "A fruit with this id alreay exists" }));

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
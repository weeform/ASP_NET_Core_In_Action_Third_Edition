using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;

// Create a WebAPplicationBuilder instance
var builder = WebApplication.CreateBuilder(args);

// Register the required services and configuration with the WebApplicationBuilder
builder.Services.AddHttpLogging(opts => opts.LoggingFields = HttpLoggingFields.RequestProperties);
builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

// Call Build() on the builder instance to create a WebApplication instance
var app = builder.Build();

// Add middleware to the WebApplication to create a pipeline
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();
}
else
{
    app.UseExceptionHandler("/error");
}

    app.UseWelcomePage("/");
app.UseStaticFiles();
app.UseRouting();

// Map the endpoints to the WebApplication
// You can define the endpoints for your app by using MapGet() anywhere in Program.cs before the call to app.Run(),
// but the calls are typically placed after the middleware pipeline definition
app.MapGet("/hello", () => "Hello World!");
app.MapGet("/person", () => new Person("Huifeng", "Li"));
app.MapGet("/error", () => "Sorry, an error occurred");
app.MapGet("/throw", (HttpContext context) => throw new Exception("This is a test exception"));

// Call Run() on the WebApplication to start the server and handle requests
app.Run();

public record Person(string FirstName, string LastName);
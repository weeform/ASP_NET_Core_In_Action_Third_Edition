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
    app.UseHttpLogging();
}

// Map the endpoints to the WebApplication
app.MapGet("/", () => "Hello World!");
app.MapGet("/person", () => new Person("Huifeng", "Li"));

// Call Run() on the WebApplication to start the server and handle requests
app.Run();

public record Person(string FirstName, string LastName);
// Create a WebAPplicationBuilder instance
var builder = WebApplication.CreateBuilder(args);

// Register the required services and configuration with the WebApplicationBuilder

// Call Build() on the builder instance to create a WebApplication instance
var app = builder.Build();

// Add middleware to the WebApplication to create a pipeline

// Map the endpoints to the WebApplication
app.MapGet("/", () => "Hello World!");

// Call Run() on the WebApplication to start the server and handle requests
app.Run();

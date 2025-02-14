using Microsoft.EntityFrameworkCore;
using Vending_Machine_App.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("VendingConApp");
if (connectionString == "InMemory")
{
    builder.Services.AddDbContext<VendingMachineDbContext>(options =>
        options.UseInMemoryDatabase("VendingMachineDb"));
}
else
{
    builder.Services.AddDbContext<VendingMachineDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Serve static files (ensure index.html is in wwwroot)
app.UseDefaultFiles(); // Looks for index.html by default
app.UseStaticFiles();  // Serves files from wwwroot

app.UseAuthorization();
app.MapControllers();

// Handle client-side routing (e.g., Angular)
app.MapFallbackToFile("index.html");

// Render port configuration
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
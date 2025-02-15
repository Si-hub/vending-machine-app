using Microsoft.EntityFrameworkCore;
using Vending_Machine_App.Models;
using Microsoft.AspNetCore.SpaServices.Extensions; // Add this using statement

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration (KEEP THIS SECTION AS IS)
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

// CORS configuration (KEEP THIS SECTION AS IS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add CORS services (THIS IS THE FIX FOR THE ICorsService ERROR)
builder.Services.AddCors(); // Add this line

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // Use CORS policy

app.UseStaticFiles();

app.UseRouting(); // Important: Add this line for routing to work

app.UseSpa(spa =>
{
    spa.Options.DefaultPage = "/index.html"; // Correct path (leading slash)

    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // Development only!
    }
});

// Render port configuration
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
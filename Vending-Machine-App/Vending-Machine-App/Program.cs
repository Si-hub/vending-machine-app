using Microsoft.EntityFrameworkCore;
using Vending_Machine_App.Models;

var builder = WebApplication.CreateBuilder(args);

// ... (Database configuration - Keep as is) ...

// CORS configuration (Keep as is)

var app = builder.Build();

// ... (Swagger configuration - Keep as is) ...

app.UseCors("AllowAll");

app.UseStaticFiles(); // Serve static files

app.UseRouting(); // Enable routing - VERY IMPORTANT

app.UseSpa(spa =>
{
    spa.Options.DefaultPage = "index.html"; // Serve index.html

    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // For local development ONLY! Remove or comment out in production
    }
});


// ... (Other middleware) ...

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
using Microsoft.EntityFrameworkCore;
using Vending_Machine_App.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get the connection string
var connectionString = builder.Configuration.GetConnectionString("VendingConApp");

// Modified database context configuration
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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

// Serve static files for Angular
app.UseDefaultFiles();  // To serve index.html by default
app.UseStaticFiles();   // Enable serving static files

app.UseAuthorization();

app.MapControllers();

// For Angular routing
app.MapFallbackToFile("index.html");

app.Run();
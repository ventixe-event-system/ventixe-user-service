// Program.cs
using Microsoft.EntityFrameworkCore;
using ventixe_user_service.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

// CORS för att tillåta frontend att prata med backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://jolly-grass-06f829703.6.azurestaticapps.net"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Skapa databasen automatiskt
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    context.Database.EnsureCreated();
}

Console.WriteLine("🚀 User Service API startar...");
Console.WriteLine("📍 Swagger UI: https://localhost:7001/swagger");
Console.WriteLine("🔗 API Base URL: http://localhost:5271/api");

app.Run();
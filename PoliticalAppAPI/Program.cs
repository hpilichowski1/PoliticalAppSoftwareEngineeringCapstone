using Microsoft.EntityFrameworkCore;
using PoliticalAppAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// DB (MySQL via Pomelo)
var conn = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseMySql(conn, new MySqlServerVersion(new Version(8,0,36)))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

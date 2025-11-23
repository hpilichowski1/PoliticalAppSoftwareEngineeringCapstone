using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PoliticalAppAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// DB (MySQL via Pomelo)
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    var serverVersion = ServerVersion.AutoDetect(conn);

    opts.UseMySql(conn, serverVersion)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information);
});

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

app.UseAuthorization();
app.MapControllers();

app.Run();

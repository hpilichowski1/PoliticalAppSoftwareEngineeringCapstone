using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PoliticalAppAPI.Data;
using PoliticalAppAPI.Services;

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

builder.Services.AddHttpClient("CongressGov", (sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["CongressGov:BaseUrl"] ?? "https://api.congress.gov";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IRepresentativeSyncService, RepresentativeSyncService>();

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

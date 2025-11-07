// File: PoliticalAppAPI/Data/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PoliticalAppAPI.Data;

namespace PoliticalAppAPI.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine(">>> DesignTimeDbContextFactory.CreateDbContext invoked");

        // Try env var first (single-command override)
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default");

        if (string.IsNullOrWhiteSpace(cs))
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var cfg = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddUserSecrets(typeof(DesignTimeDbContextFactory).Assembly, optional: true)
                .AddEnvironmentVariables()
                .Build();

            cs = cfg.GetConnectionString("Default");
        }

        // Last resort (TEMP) to rule out config loading
        if (string.IsNullOrWhiteSpace(cs))
            cs = "Server=127.0.0.1;Port=3307;Database=politicalapp;User Id=politicalapp;Password=PoliticalApp;TreatTinyAsBoolean=true;AllowPublicKeyRetrieval=True;SslMode=Preferred";

        var sv = new MySqlServerVersion(new Version(8,0,36)); // avoid AutoDetect during design time

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(cs, sv)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDbContext(opts);
    }
}

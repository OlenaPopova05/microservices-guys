using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WorkflowService.Infrastructure.Persistence;

public class WorkflowDbContextFactory : IDesignTimeDbContextFactory<WorkflowDbContext>
{
    public WorkflowDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var apiProjectDirectory = FindApiProjectDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("WorkflowDb")
                               ?? "Host=localhost;Port=5432;Database=workflowdb;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<WorkflowDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new WorkflowDbContext(optionsBuilder.Options);
    }

    private static string FindApiProjectDirectory()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            var csproj = Path.Combine(directory.FullName, "WorkflowService.Api.csproj");
            if (File.Exists(csproj))
                return directory.FullName;

            directory = directory.Parent;
        }

        return Directory.GetCurrentDirectory();
    }
}

using FluentMigrator.Runner;

namespace HubTo.WebApi.Extensions;

public static class InitializeMigrationExtensions
{
    public static async Task<WebApplication> InitializePersistenceAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            app.Logger.LogInformation("Running migrations...");

            runner.MigrateUp();
        }

        return app;
    }
}

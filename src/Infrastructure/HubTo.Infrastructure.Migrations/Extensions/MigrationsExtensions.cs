using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HubTo.Infrastructure.Migrations.Extensions;

public static class MigrationsExtensions
{
    public static IServiceCollection AddMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                {
                    var provider = configuration["DatabaseType:Provider"]?.ToLower();
                    var conn = configuration.GetConnectionString("DefaultConnection");

                    if (provider == "postgresql")
                    {
                        rb.AddPostgres()
                          .WithGlobalConnectionString(conn);
                    }
                    else if (provider == "sqlite")
                    {
                        rb.AddSQLite()
                          .WithGlobalConnectionString(conn);
                    }
                    else
                    {
                        throw new Exception("Unsupported database provider");
                    }

                    rb.ScanIn(typeof(Init).Assembly).For.Migrations();
                });

        return services;
    }
}

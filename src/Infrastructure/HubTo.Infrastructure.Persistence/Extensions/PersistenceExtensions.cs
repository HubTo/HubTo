using HubTo.Core.Application.Contracts.Persistence;
using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Infrastructure.Persistence.Context;
using HubTo.Infrastructure.Persistence.Interceptors;
using HubTo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HubTo.Infrastructure.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseType = configuration["DatabaseType:Provider"]?.Trim().ToLower()
            ?? throw new InvalidOperationException("Database provider is not configured");

        var conn = configuration.GetConnectionString("DefaultConnection");
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<HubToContext>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();

            switch (databaseType)
            {
                case "postgresql":
                    options.UseNpgsql(conn)
                           .AddInterceptors(auditInterceptor);
                    break;

                case "sqlite":
                    options.UseSqlite(conn)
                           .AddInterceptors(auditInterceptor);
                    break;

                default:
                    throw new NotSupportedException($"Database provider '{databaseType}' not supported");
            }
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>))
                .AddScoped<IApiKeyRepository, ApiKeyRepository>()
                .AddScoped<INamespaceRepository, NamespaceRepository>()
                .AddScoped<IArtifactRepository, ArtifactRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserNamespaceRepository, UserNamespaceRepository>()
                .AddScoped<IPluginRepository, PluginRepository>()
                .AddScoped<IPluginSettingRepository, PluginSettingRepository>();

        return services;
    }
}
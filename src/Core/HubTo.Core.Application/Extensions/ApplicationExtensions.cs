using FluentValidation;
using HubTo.Core.Application.Common.Helpers.Argon2Helper;
using HubTo.Core.Application.Common.Helpers.JwtHelper;
using HubTo.Core.Application.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HubTo.Core.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assm = Assembly.GetExecutingAssembly();

        services.AddScoped<IArgon2Helper, Argon2Helper>()
                .AddScoped<IJwtHelper, JwtHelper>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<Argon2Settings>(configuration.GetSection("Argon2"));

        services.AddValidatorsFromAssembly(assm, includeInternalTypes: true);

        services.AddMediatrRegistry();

        return services;
    }
}

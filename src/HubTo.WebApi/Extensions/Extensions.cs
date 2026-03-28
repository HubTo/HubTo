using HubTo.Core.Application.Contracts;
using HubTo.WebApi.Common;
using HubTo.WebApi.Context;
using Microsoft.AspNetCore.Mvc;

namespace HubTo.WebApi.Extensions;

internal static class Extensions
{
    internal static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<ApiResponseFactory>();
        services.AddScoped<IClientContext, HttpClientContext>();
        services.AddJwtAuthentication(configuration);

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var factory = context.HttpContext.RequestServices.GetRequiredService<ApiResponseFactory>();

                var errors = new[] { "Invalid request payload." };
                var response = factory.Failure<object?>(context.HttpContext, errors, message: "");

                return new BadRequestObjectResult(response);
            };
        }); ;


        return services;
    }
}

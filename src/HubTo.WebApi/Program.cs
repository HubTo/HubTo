using HubTo.Core.Application.Extensions;
using HubTo.Infrastructure.Migrations.Extensions;
using HubTo.Infrastructure.Persistence.Extensions;
using HubTo.Infrastructure.Plugins.Extensions;
using HubTo.WebApi.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging();

builder.Services.AddControllers();

builder.Services.AddApplication(builder.Configuration)
                .AddPersistence(builder.Configuration)
                .AddMigrations(builder.Configuration)
                .AddPluginsServices()
                .AddWebApi(builder.Configuration);
// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();

await app.InitializePersistenceAsync();
await app.AddPlugins();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("HubTo API Documentation")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}
app.AddMiddlewares();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

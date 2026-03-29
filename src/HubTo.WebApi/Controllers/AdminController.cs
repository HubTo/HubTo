using Azure.Core;
using HubTo.Abstraction.Registrars;
using HubTo.Core.Application.Common.Results;
using HubTo.Core.Application.Common.Settings;
using HubTo.Core.Application.Contracts.Plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HubTo.WebApi.Controllers;

[ApiController]
[Route("admin/plugins")]
public class PluginAdminController : ControllerBase
{
    private readonly IPluginLoader _pluginLoader;
    private readonly IPluginRegistry _registry;
    private readonly AdminSettings _settings;

    public PluginAdminController(
        IPluginLoader pluginLoader, 
        IPluginRegistry registry, 
        IOptions<AdminSettings> settings)
    {
        _pluginLoader = pluginLoader;
        _registry = registry;
        _settings = settings.Value;
    }

    private bool IsAuthorized()
    {
        Request.Headers.TryGetValue("X-Admin-Key", out var providedKey);
        return !string.IsNullOrWhiteSpace(_settings.Key) && _settings.Key == providedKey;
    }

    [HttpGet]
    public IActionResult List()
    {
        if (!IsAuthorized()) return Unauthorized();

        var plugins = _registry.AllPlugins.Select(p => new
        {
            p.Name,
            Type = p is IRegistrarPlugin r ? r.RegistrarType : "Storage"
        });

        return Ok(plugins);
    }

    [HttpPost("reload")]
    public async Task<IActionResult> Reload(CancellationToken ct)
    {
        if (!IsAuthorized()) return Unauthorized();

        var result = await _pluginLoader.ReloadPluginAsync(ct);
        return result ? Ok(new { message = " reloaded." }) : BadRequest(new { error = "Failed to reload plugin." });
    }

    [HttpGet("health")]
    public async Task<IActionResult> Health(CancellationToken ct)
    {
        if (!IsAuthorized()) return Unauthorized();

        var results = new List<PluginHealthResult>();

        foreach (var plugin in _registry.AllPlugins)
        {
            var health = await plugin.CheckHealthAsync(ct);
            results.Add(new PluginHealthResult(plugin.Name, health.IsHealthy, health.Details));
        }

        var allHealthy = results.All(r => r.IsHealthy);
        return StatusCode(allHealthy ? 200 : 207, results);
    }
}
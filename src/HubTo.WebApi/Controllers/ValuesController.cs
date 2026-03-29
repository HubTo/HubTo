using HubTo.Abstraction.Models;
using HubTo.Abstraction.Registrars;
using HubTo.Core.Application.Contracts.Plugins;
using Microsoft.AspNetCore.Mvc;

namespace HubTo.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IStoragePlugin _storagePlugin;

    public ValuesController(IPluginRegistry registry)
    {
        _storagePlugin = registry.GetDefaultStorage();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Save(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Dosya boş.");

        await using var stream = file.OpenReadStream();

        var fileId = await _storagePlugin.SaveAsync(
            stream,
            file.FileName,
            new StorageMetadata
            {
                ContentType = file.ContentType,
                Tags = new Dictionary<string, string>()
            },
            cancellationToken);

        return Ok(new
        {
            FileId = fileId,
            FileName = file.FileName,
            Size = file.Length
        });
    }
}

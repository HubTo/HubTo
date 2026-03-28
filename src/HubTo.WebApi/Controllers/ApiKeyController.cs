using HubTo.Core.Application.Common;
using HubTo.Core.Application.Features.ApiKey.Commands.Create;
using HubTo.WebApi.Controllers.Base;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HubTo.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ApiKeyController : BaseController
{
    public ApiKeyController(IMediator mediator) : base(mediator) { }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        Result<CreateApiKeyDto> result = await _mediator.Send(request, cancellationToken);

        return ProcessResult(result);
    }
}

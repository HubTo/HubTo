using HubTo.Core.Application.Common.Results;
using HubTo.Core.Application.Features.Namespace.Commands.Create;
using HubTo.WebApi.Controllers.Base;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ILBER.WebApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
public class NamespaceController : BaseController
{
    public NamespaceController(IMediator mediator) : base(mediator) { }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateNamespaceCommand request, CancellationToken cancellationToken)
    {
        Result<CreateNamespaceDto> result = await _mediator.Send(request, cancellationToken);

        return ProcessResult(result);
    }
}

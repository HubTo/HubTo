using HubTo.Core.Application.Common.Results;
using HubTo.Core.Application.Features.Auth.Commands.Login;
using HubTo.Core.Application.Features.Auth.Commands.Register;
using HubTo.WebApi.Controllers.Base;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace HubTo.WebApi.Controllers;

[Route("api/v1/[controller]")]
public class AuthController : BaseController
{
    public AuthController(IMediator mediator) : base(mediator) { }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken cancellationToken)
    {
        Result<RegisterDto> result = await _mediator.Send(request, cancellationToken);

        return ProcessResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken)
    {
        Result<LoginDto> result = await _mediator.Send(request, cancellationToken);

        return ProcessResult(result);
    }
}

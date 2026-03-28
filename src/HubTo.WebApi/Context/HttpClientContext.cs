using HubTo.Core.Application.Contracts;
using System.Security.Claims;

namespace HubTo.WebApi.Context;

internal sealed class HttpClientContext : IClientContext
{
    private readonly IHttpContextAccessor _accessor;
    public HttpClientContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid? UserId => Guid.TryParse(_accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public string TraceId => _accessor.HttpContext?.TraceIdentifier ?? "N/A";
}

using HubTo.Core.Application.Common.Results;
using HubTo.WebApi.Common;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace HubTo.WebApi.Controllers.Base;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private ApiResponseFactory? _responseFactory;
    protected ApiResponseFactory ResponseFactory =>
        _responseFactory ??= HttpContext.RequestServices.GetRequiredService<ApiResponseFactory>();

    protected readonly IMediator _mediator;

    protected BaseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected IActionResult ProcessResult<T>(Result<T> result, int failureStatusCode = StatusCodes.Status400BadRequest)
    {
        if (result.IsSuccess)
        {
            return Ok(ResponseFactory.Success(HttpContext, result.Value));
        }

        return StatusCode(
            failureStatusCode,
            ResponseFactory.Failure(
                HttpContext,
                result.Errors,
                result.Data,
                "An error occurred."));
    }

    protected IActionResult ProcessResult(Result result, int failureStatusCode = StatusCodes.Status400BadRequest)
    {
        if (result.IsSuccess)
        {
            return Ok(ResponseFactory.Success<object?>(HttpContext, null));
        }

        return StatusCode(
            failureStatusCode,
            ResponseFactory.Failure<object?>(
                HttpContext,
                result.Errors,
                message: "An error occurred."));
    }
}

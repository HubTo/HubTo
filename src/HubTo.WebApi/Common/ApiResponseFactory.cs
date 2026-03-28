namespace HubTo.WebApi.Common;

public sealed class ApiResponseFactory
{
    public ApiResponse<T> Success<T>(HttpContext context, T? data)
    {
        return ApiResponse<T>.Ok(GetTraceId(context), data);
    }

    public ApiResponse<T> Failure<T>(
        HttpContext context,
        IEnumerable<string> errors,
        T? data = default,
        string? message = null)
    {
        var errorList = errors?.ToList() ?? new List<string> { "An error occurred." };

        return ApiResponse<T>.Fail(
            GetTraceId(context),
            message ?? "Operation failed.",
            errorList,
            data);
    }

    public ApiResponse<object?> UnexpectedFailure(HttpContext context)
    {
        return ApiResponse<object?>.Fail(
            GetTraceId(context),
            "Unexpected Error",
            new List<string> { "An unexpected error occurred on the server." });
    }

    private static string GetTraceId(HttpContext context)
    {
        if (System.Diagnostics.Activity.Current != null)
        {
            return System.Diagnostics.Activity.Current.TraceId.ToHexString();
        }

        var traceIdentifier = context.TraceIdentifier;
        if (traceIdentifier.Contains("-"))
        {
            var parts = traceIdentifier.Split('-');
            return parts.Length > 2 ? parts[1] : traceIdentifier;
        }

        return traceIdentifier;
    }
}

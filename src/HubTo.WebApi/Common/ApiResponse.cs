namespace HubTo.WebApi.Common;

public sealed record ApiResponse<T>(
    bool Success,
    string? Message,
    IReadOnlyList<string> Errors,
    string TraceId,
    T? Data)
{
    public static ApiResponse<T> Ok(string traceId, T? data) =>
        new(true, null, [], traceId, data);

    public static ApiResponse<T> Fail(
        string traceId,
        string message,
        IReadOnlyList<string> errors,
        T? data = default) => new(false, message, errors, traceId, data);
}
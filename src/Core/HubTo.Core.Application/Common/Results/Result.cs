namespace HubTo.Core.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<string> Errors { get; }
    public object? Data { get; }

    protected Result(bool success, IEnumerable<string>? errors = null, object? data = null)
    {
        IsSuccess = success;
        Errors = errors?.ToArray() ?? Array.Empty<string>();
        Data = data;
    }

    public static Result Ok() => new(true);
    public static Result Fail(string error) => new(false, new[] { error });
    public static Result Fail(IEnumerable<string> errors) => new(false, errors);
    public static Result Fail(IEnumerable<string> errors, object data) => new(false, errors, data);
    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);
    public static Result<T> Fail<T>(params string[] errors) => Result<T>.Fail(errors);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, data: value)
    {
        Value = value;
    }

    private Result(IEnumerable<string> errors, object? data = null) : base(false, errors, data)
    {
        Value = default;
    }

    public static Result<T> Ok(T value) => new(value);
    public new static Result<T> Fail(params string[] errors) => new(errors);
    public new static Result<T> Fail(IEnumerable<string> errors, object? data = null) => new(errors, data);
}
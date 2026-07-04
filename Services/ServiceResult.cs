namespace graphql_proj_Csharp.Services;

public enum ServiceErrorType
{
    None,
    NotFound,
    Validation,
    Conflict,
    Unauthorized
}

public sealed record ServiceResult<T>(T? Value, ServiceErrorType ErrorType = ServiceErrorType.None, string? ErrorMessage = null)
{
    public bool Succeeded => ErrorType == ServiceErrorType.None;

    public static ServiceResult<T> Success(T value) => new(value);

    public static ServiceResult<T> NotFound(string message) => new(default, ServiceErrorType.NotFound, message);

    public static ServiceResult<T> Validation(string message) => new(default, ServiceErrorType.Validation, message);

    public static ServiceResult<T> Conflict(string message) => new(default, ServiceErrorType.Conflict, message);

    public static ServiceResult<T> Unauthorized(string message) => new(default, ServiceErrorType.Unauthorized, message);
}

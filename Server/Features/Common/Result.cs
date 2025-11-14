namespace ICTAce.FileHub.Features.Common;

/// <summary>
/// Represents the result of an operation with success/failure state
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public ErrorType ErrorType { get; private set; }

    private Result(bool isSuccess, T? value, string? error, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new(true, value, null, ErrorType.None);
    
    public static Result<T> Failure(string error, ErrorType errorType = ErrorType.Validation) 
        => new(false, default, error, errorType);
    
    public static Result<T> NotFound(string error) 
        => new(false, default, error, ErrorType.NotFound);
    
    public static Result<T> Unauthorized(string error) 
        => new(false, default, error, ErrorType.Unauthorized);
}

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Unauthorized,
    Conflict
}

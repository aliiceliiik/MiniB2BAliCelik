namespace MiniB2B.Entities.Dtos.Common;

public class ServiceResult
{
    public bool IsSuccess { get; protected init; }
    public string? ErrorMessage { get; protected init; }

    public static ServiceResult Success() => new() { IsSuccess = true };

    public static ServiceResult Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private init; }

    public static ServiceResult<T> Success(T data) =>
        new() { IsSuccess = true, Data = data };

    public static new ServiceResult<T> Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
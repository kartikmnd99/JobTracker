namespace JobTracker.Application.DTOs;

public enum ResultStatus { Ok, NotFound, Invalid }

public class ServiceResult<T>
{
    public ResultStatus Status { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }

    public static ServiceResult<T> Ok(T data) => new() { Status = ResultStatus.Ok, Data = data };
    public static ServiceResult<T> NotFound() => new() { Status = ResultStatus.NotFound };
    public static ServiceResult<T> Invalid(string error) => new() { Status = ResultStatus.Invalid, Error = error };
}
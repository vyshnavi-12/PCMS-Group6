namespace PCMS_Backend.Shared;

public class Result<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public string? Token { get; set; }

    public static Result<T> Ok(T data) => new() { Success = true, Data = data, StatusCode = StatusCodes.Status200OK };
    public static Result<T> Ok(T data, string message, string token) => new() { Success = true, Message = message, Data = data, StatusCode = StatusCodes.Status200OK, Token = token };
    public static Result<T> Ok(T data, string message) => new() { Success = true, Message = message, Data = data, StatusCode = StatusCodes.Status200OK };

    public static Result<T> NotFound(string message) =>
        new() { Success = false, Message = message, StatusCode = StatusCodes.Status404NotFound };
    public static Result<T> Forbidden(string message) =>
        new() { Success = false, Message = message, StatusCode = StatusCodes.Status403Forbidden };
    public static Result<T> Unauthorized(string message = "Unauthorized") =>
        new() { Success = false, Message = message, StatusCode = StatusCodes.Status401Unauthorized };
    public static Result<T> BadRequest(string message) =>
            new() { Success = false, Message = message, StatusCode = StatusCodes.Status400BadRequest };

    public static Result<T> ServerError(string message = "Internal server error") =>
          new() { Success = false, Message = message, StatusCode = StatusCodes.Status500InternalServerError };

}
namespace PCMS_Backend.Shared;

public class Result
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
    public int? StatusCode { get; set; }

    public Result() { }

    public static Result Ok(string message) =>
            new() { Success = true, Message = message, StatusCode = StatusCodes.Status200OK };

    public static Result NoContent() =>
            new() { Success = true, StatusCode = StatusCodes.Status204NoContent };

    public static Result NotFound(string message) =>
            new() { Success = false, Message = message, StatusCode = StatusCodes.Status404NotFound };

    public static Result BadRequest(string message) =>
            new() { Success = false, Message = message, StatusCode = StatusCodes.Status400BadRequest };

    public static Result Forbidden(string message) =>
            new() { Success = false, Message = message, StatusCode = StatusCodes.Status403Forbidden };

    public static Result Conflict(string message) =>
            new() { Success = false, Message = message, StatusCode = StatusCodes.Status409Conflict };

    public static Result Created(string message) =>
            new() { Success = true, Message = message, StatusCode = StatusCodes.Status201Created };

    public static Result Unauthorized(string message = "Unauthorized") =>
            new() { Success = false, Message = message, StatusCode = StatusCodes.Status401Unauthorized };
}

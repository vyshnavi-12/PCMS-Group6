using Microsoft.AspNetCore.Mvc;

namespace PCMS_Backend.Shared;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {

        return result.StatusCode switch
        {
            StatusCodes.Status200OK =>
                new OkObjectResult(new { message = result.Message }),

            StatusCodes.Status204NoContent =>
                new NoContentResult(),

            StatusCodes.Status400BadRequest =>
                new BadRequestObjectResult(new { message = result.Message }),

            StatusCodes.Status401Unauthorized =>
                new UnauthorizedObjectResult(new { message = result.Message }),

            StatusCodes.Status403Forbidden =>
                new ObjectResult(new { message = result.Message }) { StatusCode = StatusCodes.Status403Forbidden },

            StatusCodes.Status404NotFound =>
                new NotFoundObjectResult(new { message = result.Message }),

            StatusCodes.Status409Conflict =>
                new ConflictObjectResult(new { message = result.Message }),

            StatusCodes.Status201Created =>
                new ObjectResult(new { message = result.Message }) { StatusCode = StatusCodes.Status201Created },

                StatusCodes.Status500InternalServerError => 
                new ObjectResult(new { message = result.Message }) { StatusCode = StatusCodes.Status500InternalServerError },

            _ => new ObjectResult(new { message = "Unknown error" }) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => new OkObjectResult(result.Message == null ? new { data = result.Data } : new { message = result.Message, data = result.Data }),
            StatusCodes.Status403Forbidden =>
                new ObjectResult(new { message = result.Message }) { StatusCode = StatusCodes.Status403Forbidden },
            StatusCodes.Status404NotFound =>
            new NotFoundObjectResult(new { message = result.Message }),
            StatusCodes.Status401Unauthorized =>
                new UnauthorizedObjectResult(new { message = result.Message }),
            StatusCodes.Status400BadRequest =>
            new BadRequestObjectResult(new { message = result.Message }),

            _ =>
                new ObjectResult(new
                {
                    message = "Internal Server Error",
                    detail = result.Message
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                }
        };
    }

}
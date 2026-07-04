using graphql_proj_Csharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

public static class ControllerResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
    {
        if (result.Succeeded && result.Value is not null)
        {
            return controller.Ok(result.Value);
        }

        return result.ErrorType switch
        {
            ServiceErrorType.NotFound => controller.NotFound(result.ErrorMessage),
            ServiceErrorType.Conflict => controller.Conflict(result.ErrorMessage),
            ServiceErrorType.Unauthorized => controller.Unauthorized(result.ErrorMessage),
            ServiceErrorType.Validation => controller.BadRequest(result.ErrorMessage),
            _ => controller.BadRequest(result.ErrorMessage)
        };
    }
}

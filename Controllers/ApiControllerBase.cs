using Microsoft.AspNetCore.Mvc;
using StancaBlogApi.Core.Common;

namespace StancaBlogApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult(ServiceResult result)
    {
        if (result.StatusCode == StatusCodes.Status204NoContent)
            return NoContent();

        if (!string.IsNullOrWhiteSpace(result.Error))
            return StatusCode(result.StatusCode, new { message = result.Error });

        return StatusCode(result.StatusCode);
    }

    protected IActionResult ToActionResult<T>(ServiceResult<T> result)
    {
        if (result.StatusCode == StatusCodes.Status204NoContent)
            return NoContent();

        if (!string.IsNullOrWhiteSpace(result.Error))
            return StatusCode(result.StatusCode, new { message = result.Error });

        return StatusCode(result.StatusCode, result.Data);
    }
}

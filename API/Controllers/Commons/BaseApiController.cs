using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace API.Controllers.Commons
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult HandleServiceResult<T>(ServiceResult<T> result)
        {
            return result.ResultType switch
            {
                ServiceResultType.Success => Ok(result.Data),
                ServiceResultType.Conflict => Conflict(new { Message = result.Message }),
                ServiceResultType.NotFound => NotFound(new { Message = result.Message }),
                ServiceResultType.Error => BadRequest(new { Message = result.Message }),
                ServiceResultType.ValidationError => UnprocessableEntity(new { Message = result.Message }),
                ServiceResultType.ServiceUnavailable => StatusCode(StatusCodes.Status503ServiceUnavailable, new { Message = result.Message }),
                _ => StatusCode(500, new { Message = "Internal server error" }),
            };
        }

        protected IActionResult HandleServiceResult(ServiceResult result)
        {
            return result.ResultType switch
            {
                ServiceResultType.Success => Ok(),
                ServiceResultType.Conflict => Conflict(new { Message = result.Message }),
                ServiceResultType.NotFound => NotFound(new { Message = result.Message }),
                ServiceResultType.Error => BadRequest(new { Message = result.Message }),
                ServiceResultType.ValidationError => UnprocessableEntity(new { Message = result.Message }),
                _ => StatusCode(500, new { Message = "Internal server error" }),
            };
        }
    }
}
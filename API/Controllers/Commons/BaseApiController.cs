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
                ServiceResultType.Success => Ok(
                    new
                    {
                        Data = result.Data,
                        Message = result.Message
                    }
                ),
                ServiceResultType.Created => StatusCode(StatusCodes.Status201Created,
                    new
                    {
                        Data = result.Data,
                        Message = result.Message
                    }
                ),
                ServiceResultType.NoContent => StatusCode(StatusCodes.Status204NoContent,
                    new
                    {
                        Data = result.Data,
                        Message = result.Message
                    }
                ),
                ServiceResultType.Conflict => Conflict(CreateError("CONFLICT", result.Message)),
                ServiceResultType.NotFound => NotFound(CreateError("NOT_FOUND", result.Message)),
                ServiceResultType.Error => BadRequest(CreateError("BAD_REQUEST", result.Message)),
                ServiceResultType.ValidationError => UnprocessableEntity(CreateError("VALIDATION_ERROR", result.Message)),
                ServiceResultType.ServiceUnavailable => StatusCode(StatusCodes.Status503ServiceUnavailable, CreateError("INTERNAL_ERROR", result.Message)),
                _ => StatusCode(500, CreateError("INTERNAL_ERROR", result.Message)),
            };
        }

        protected IActionResult HandleServiceResult(ServiceResult result)
        {
            return result.ResultType switch
            {
                ServiceResultType.Success => Ok(),
                ServiceResultType.Conflict => Conflict(CreateError("CONFLICT", result.Message)),
                ServiceResultType.NotFound => NotFound(CreateError("NOT_FOUND", result.Message)),
                ServiceResultType.Error => BadRequest(CreateError("BAD_REQUEST", result.Message)),
                ServiceResultType.ValidationError => UnprocessableEntity(CreateError("VALIDATION_ERROR", result.Message)),
                ServiceResultType.ServiceUnavailable => StatusCode(StatusCodes.Status503ServiceUnavailable, CreateError("INTERNAL_ERROR", result.Message)),
                _ => StatusCode(500, CreateError("INTERNAL_ERROR", result.Message)),
            };
        }

        private object CreateError(string code, string? message)
        => new
        {
            Code = code,
            Message = message ?? "An error occurred",
            TraceId = HttpContext.TraceIdentifier
        };
    }
}
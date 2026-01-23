namespace Shared.Results
{
    public enum ServiceResultType
    {
        Success,
        Created,
        NoContent,
        NotFound,
        Error,
        BadRequest,
        Unauthorized,
        Forbidden,
        ValidationError,
        InternalServerError,
        Conflict,
        ServiceUnavailable
    }
}

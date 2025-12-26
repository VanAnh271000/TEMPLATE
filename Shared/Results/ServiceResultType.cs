namespace Shared.Results
{
    public enum ServiceResultType
    {
        Success,
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

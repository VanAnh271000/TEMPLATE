namespace Shared.Results
{
    public class ServiceResult
    {
        public ServiceResultType ResultType { get; protected set; }
        public string Message { get; protected set; }
        public bool IsSuccess => ResultType == ServiceResultType.Success;

        public static ServiceResult Success() => new() { ResultType = ServiceResultType.Success };
        public static ServiceResult NotFound(string message) => new() { ResultType = ServiceResultType.NotFound, Message = message };
        public static ServiceResult Error(string message) => new() { ResultType = ServiceResultType.Error, Message = message };
        public static ServiceResult BadRequest(string message) => new() { ResultType = ServiceResultType.BadRequest, Message = message };
        public static ServiceResult ValidationError(string message) => new() { ResultType = ServiceResultType.ValidationError, Message = message };
        public static ServiceResult Unauthorized(string message) => new() { ResultType = ServiceResultType.Unauthorized, Message = message };
        public static ServiceResult Forbidden(string message) => new() { ResultType = ServiceResultType.Forbidden, Message = message };
        public static ServiceResult Conflict(string message) => new() { ResultType = ServiceResultType.Conflict, Message = message };
        public static ServiceResult InternalServerError(string message) => new() { ResultType = ServiceResultType.InternalServerError, Message = message };
        
    }
    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; private set; }
        public static ServiceResult<T> Success(T data) => new() { ResultType = ServiceResultType.Success, Data = data };
        public new static ServiceResult<T> NotFound(string message) => new() { ResultType = ServiceResultType.NotFound, Message = message };
        public new static ServiceResult<T> Error(string message) => new() { ResultType = ServiceResultType.Error, Message = message };
        public new static ServiceResult<T> BadRequest(string message) => new() { ResultType = ServiceResultType.BadRequest, Message = message };
        public new static ServiceResult<T> ValidationError(string message) => new() { ResultType = ServiceResultType.ValidationError, Message = message };
        public new static ServiceResult<T> Unauthorized(string message) => new() { ResultType = ServiceResultType.Unauthorized, Message = message };
        public new static ServiceResult<T> Forbidden(string message) => new() { ResultType = ServiceResultType.Forbidden, Message = message };
        public new static ServiceResult<T> Conflict(string message) => new() { ResultType = ServiceResultType.Conflict, Message = message };
        public new static ServiceResult<T> InternalServerError(string message) => new() { ResultType = ServiceResultType.Error, Message = message };
    }

    
}

namespace Application.Exceptions
{
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message) { }
    }

    public sealed class AppNotFoundException : AppException
    {
        public AppNotFoundException(string message) : base(message) { }
    }

    public sealed class AppConflictException : AppException
    {
        public AppConflictException(string message) : base(message) { }
    }
    
    public sealed class AppValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public AppValidationException(
            IDictionary<string, string[]> errors)
            : base("Request validation failed")
        {
            Errors = errors;
        }
    }

}

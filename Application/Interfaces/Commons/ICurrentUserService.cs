namespace Application.Interfaces.Commons
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string? FullName { get; }
    }
}

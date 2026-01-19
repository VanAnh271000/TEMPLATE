namespace Domain.Entities.Identity
{
    public class FirebaseToken
    {
        public int Id { get; init; }
        public Guid UserId { get; init; }
        public string DeviceToken { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
        public bool IsActive { get; set; }
    }
}

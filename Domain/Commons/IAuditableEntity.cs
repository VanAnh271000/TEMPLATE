namespace Domain.Commons
{
    public interface IAuditableEntity
    {
        DateTime? CreatedTime { get; set; }
        string? CreatedById { get; set; }
        string? CreatedByName { get; set; }
        DateTime? UpdatedTime { get; set; }
        string? UpdatedById { get; set; }
        string? UpdatedByName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

}

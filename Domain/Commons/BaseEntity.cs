using System.ComponentModel.DataAnnotations;
namespace Domain.Commons
{
    public abstract class BaseEntity<T> : IAuditableEntity
    {
        [Key]
        public T Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedTime { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? UpdatedById { get; set; }
        public string? UpdatedByName { get; set; }
    }
}

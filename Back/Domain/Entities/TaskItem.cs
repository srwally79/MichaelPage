namespace Domain.Entities;

public sealed class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid StatusId { get; set; }
    public Guid UserId { get; set; }
    public string AdditionalInfo { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public Status Status { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<Log> Logs { get; set; } = new List<Log>();
}

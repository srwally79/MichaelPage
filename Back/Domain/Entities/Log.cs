namespace Domain.Entities;

public sealed class Log
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
    public TaskItem Task { get; set; } = null!;
}

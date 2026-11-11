namespace Domain.Enums;

public static class TaskStatusNames
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Done = "Done";
    public static readonly string[] All = [Pending, InProgress, Done];
}

using System.Text.Json;

namespace Application.DTOs;

public sealed record CreateTaskRequest(string? Title, Guid UserId, Dictionary<string, JsonElement>? AdditionalInfo);
public sealed record UpdateTaskStatusRequest(string? Status);
public sealed record TaskResponse(
    Guid Id,
    string Title,
    Guid UserId,
    string UserName,
    Guid StatusId,
    string Status,
    Dictionary<string, JsonElement> AdditionalInfo,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? FinishedAt);
public sealed record StatusResponse(Guid Id, string Name);
public sealed record LogResponse(Guid Id, Guid TaskId, DateTime CreatedAt, string? Description);

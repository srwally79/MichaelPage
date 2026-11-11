using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Repositories;

namespace Application.Services;

public sealed class TaskService(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    IStatusRepository statusRepository,
    ILogRepository logRepository) : ITaskService
{
    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var title = request.Title?.Trim();
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessRuleException("Task title is required.");
        if (title.Length > 255)
            throw new BusinessRuleException("Task title must be at most 255 characters.");
        if (request.UserId == Guid.Empty || await userRepository.GetByIdAsync(request.UserId, cancellationToken) is not { } user)
            throw new EntityNotFoundException("Assigned user was not found.");

        var info = JsonSerializer.Serialize(request.AdditionalInfo ?? new Dictionary<string, JsonElement>());
        var status = await statusRepository.GetByNameAsync(TaskStatusNames.Pending, cancellationToken)
            ?? throw new InvalidOperationException("The Pending status is not configured.");
        var now = DateTime.UtcNow;
        var task = new TaskItem
        {
            Id = Guid.NewGuid(), Title = title, UserId = user.Id, User = user,
            StatusId = status.Id, Status = status, AdditionalInfo = info,
            CreatedAt = now
        };

        await taskRepository.AddAsync(task, cancellationToken);
        await logRepository.AddAsync(new Log
        {
            Id = Guid.NewGuid(), TaskId = task.Id, Task = task,
            CreatedAt = now, Description = "Task created with Pending status."
        }, cancellationToken);
        return Map(task);
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(Guid? userId, string? status, string? priority, CancellationToken cancellationToken = default)
    {
        var normalizedStatus = status?.Trim();
        if (!string.IsNullOrEmpty(normalizedStatus) && !TaskStatusNames.All.Contains(normalizedStatus, StringComparer.OrdinalIgnoreCase))
            throw new BusinessRuleException("Status must be Pending, InProgress, or Done.");
        var normalizedPriority = priority?.Trim();
        if (normalizedPriority?.Length > 50)
            throw new BusinessRuleException("Priority must be at most 50 characters.");

        var tasks = await taskRepository.GetAllAsync(userId, normalizedStatus, normalizedPriority, cancellationToken);
        return tasks.Select(Map).ToList();
    }

    public async Task<TaskResponse> UpdateStatusAsync(Guid id, UpdateTaskStatusRequest request, CancellationToken cancellationToken = default)
    {
        var requestedStatus = request.Status?.Trim();
        if (string.IsNullOrWhiteSpace(requestedStatus) || !TaskStatusNames.All.Contains(requestedStatus, StringComparer.OrdinalIgnoreCase))
            throw new BusinessRuleException("Status must be Pending, InProgress, or Done.");

        var task = await taskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException("Task was not found.");
        var status = await statusRepository.GetByNameAsync(requestedStatus, cancellationToken)
            ?? throw new InvalidOperationException("The requested status is not configured.");
        var previousStatusName = task.Status.Name;
        if (string.Equals(previousStatusName, status.Name, StringComparison.OrdinalIgnoreCase))
            return Map(task);
        if (previousStatusName == TaskStatusNames.Pending && status.Name == TaskStatusNames.Done)
            throw new BusinessRuleException("A task cannot move directly from Pending to Done.");

        var now = DateTime.UtcNow;
        if (status.Name == TaskStatusNames.InProgress && task.StartedAt is null)
            task.StartedAt = now;
        if (status.Name == TaskStatusNames.Done)
            task.FinishedAt = now;
        task.StatusId = status.Id;
        task.Status = status;
        await taskRepository.UpdateAsync(task, cancellationToken);
        await logRepository.AddAsync(new Log
        {
            Id = Guid.NewGuid(), TaskId = task.Id, Task = task,
            CreatedAt = now, Description = $"Task status changed from {previousStatusName} to {status.Name}."
        }, cancellationToken);
        return Map(task);
    }

    private static TaskResponse Map(TaskItem task)
    {
        var additionalInfo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(task.AdditionalInfo)
            ?? new Dictionary<string, JsonElement>();
        return new TaskResponse(
            task.Id, task.Title, task.UserId, task.User.Name, task.StatusId, task.Status.Name,
            additionalInfo, task.CreatedAt, task.StartedAt, task.FinishedAt);
    }
}

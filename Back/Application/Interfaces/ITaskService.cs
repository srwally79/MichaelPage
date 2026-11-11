using Application.DTOs;

namespace Application.Interfaces;

public interface ITaskService
{
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(Guid? userId, string? status, CancellationToken cancellationToken = default);
    Task<TaskResponse> UpdateStatusAsync(Guid id, UpdateTaskStatusRequest request, CancellationToken cancellationToken = default);
}

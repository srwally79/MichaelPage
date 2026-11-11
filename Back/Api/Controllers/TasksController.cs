using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public sealed class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await taskService.CreateAsync(request, cancellationToken);
        return Created("/api/tasks", task);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetAll(
        [FromQuery] string? status, [FromQuery] Guid? userId, [FromQuery] string? priority, CancellationToken cancellationToken) =>
        Ok(await taskService.GetAllAsync(userId, status, priority, cancellationToken));

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<TaskResponse>> UpdateStatus(
        Guid id, UpdateTaskStatusRequest request, CancellationToken cancellationToken) =>
        Ok(await taskService.UpdateStatusAsync(id, request, cancellationToken));
}

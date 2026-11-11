using Application.DTOs;
using Application.Interfaces;
using Domain.Repositories;

namespace Application.Services;

public sealed class LogService(ILogRepository logRepository) : ILogService
{
    public async Task<IReadOnlyList<LogResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var logs = await logRepository.GetAllAsync(cancellationToken);
        return logs.Select(log => new LogResponse(log.Id, log.TaskId, log.CreatedAt, log.Description)).ToList();
    }
}

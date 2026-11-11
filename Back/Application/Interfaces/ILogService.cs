using Application.DTOs;

namespace Application.Interfaces;

public interface ILogService
{
    Task<IReadOnlyList<LogResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}

using Domain.Entities;

namespace Domain.Repositories;

public interface ILogRepository
{
    Task<IReadOnlyList<Log>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Log log, CancellationToken cancellationToken = default);
}

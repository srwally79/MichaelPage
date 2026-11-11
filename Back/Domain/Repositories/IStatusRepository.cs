using Domain.Entities;

namespace Domain.Repositories;

public interface IStatusRepository
{
    Task<Status?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Status>> GetAllAsync(CancellationToken cancellationToken = default);
}

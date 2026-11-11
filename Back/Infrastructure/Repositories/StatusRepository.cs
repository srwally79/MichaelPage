using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class StatusRepository(AppDbContext dbContext) : IStatusRepository
{
    public Task<Status?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        dbContext.Statuses.FirstOrDefaultAsync(status => status.Name == name, cancellationToken);

    public async Task<IReadOnlyList<Status>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Statuses.AsNoTracking().OrderBy(status => status.Name).ToListAsync(cancellationToken);
}

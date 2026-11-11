using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class LogRepository(AppDbContext dbContext) : ILogRepository
{
    public async Task<IReadOnlyList<Log>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Logs.AsNoTracking().OrderByDescending(log => log.CreatedAt).ToListAsync(cancellationToken);

    public async Task AddAsync(Log log, CancellationToken cancellationToken = default)
    {
        dbContext.Logs.Add(log);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

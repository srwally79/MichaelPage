using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class TaskRepository(AppDbContext dbContext) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Tasks.Include(task => task.User).Include(task => task.Status)
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid? userId, string? status, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tasks.AsNoTracking().Include(task => task.User).Include(task => task.Status).AsQueryable();
        if (userId.HasValue)
            query = query.Where(task => task.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(task => task.Status.Name == status);
        return await query.OrderByDescending(task => task.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}

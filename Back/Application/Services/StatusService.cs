using Application.DTOs;
using Application.Interfaces;
using Domain.Repositories;

namespace Application.Services;

public sealed class StatusService(IStatusRepository statusRepository) : IStatusService
{
    public async Task<IReadOnlyList<StatusResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var statuses = await statusRepository.GetAllAsync(cancellationToken);
        return statuses.Select(status => new StatusResponse(status.Id, status.Name)).ToList();
    }
}

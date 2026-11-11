using Application.DTOs;

namespace Application.Interfaces;

public interface IStatusService
{
    Task<IReadOnlyList<StatusResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}

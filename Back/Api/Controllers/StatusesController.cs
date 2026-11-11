using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/statuses")]
[Authorize]
public sealed class StatusesController(IStatusService statusService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StatusResponse>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await statusService.GetAllAsync(cancellationToken));
}

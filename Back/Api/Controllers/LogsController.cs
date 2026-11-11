using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/logs")]
[Authorize]
public sealed class LogsController(ILogService logService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LogResponse>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await logService.GetAllAsync(cancellationToken));
}

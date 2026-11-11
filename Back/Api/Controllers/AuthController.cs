using Application.DTOs;
using Application.Interfaces;
using Api.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var login = await authService.LoginAsync(request, cancellationToken);
        Response.Cookies.Append(AuthCookie.Name, login.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = AuthCookie.Path,
            IsEssential = true,
            Expires = new DateTimeOffset(DateTime.SpecifyKind(login.ExpiresAt, DateTimeKind.Utc))
        });
        return Ok(new LoginResponse(login.ExpiresAt));
    }

    [HttpGet("session")]
    public IActionResult GetSession() => Ok(new { username = User.Identity?.Name });

    [HttpPost("logout")]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthCookie.Name, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = AuthCookie.Path
        });
        return NoContent();
    }
}

using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Repositories;

namespace Application.Services;

public sealed class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService) : IAuthService
{
    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new BusinessRuleException("Username or password is invalid.");

        var user = await userRepository.GetByNameAsync(request.Username.Trim(), cancellationToken);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new BusinessRuleException("Username or password is invalid.");

        var result = tokenService.CreateToken(user);
        return new LoginResult(result.Token, result.ExpiresAt);
    }
}

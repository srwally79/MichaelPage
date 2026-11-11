namespace Application.DTOs;

public sealed record CreateUserRequest(string? Name, string? Email, string? Password);
public sealed record UserResponse(Guid Id, string Name, string Email);
public sealed record LoginRequest(string? Username, string? Password);
public sealed record LoginResult(string Token, DateTime ExpiresAt);
public sealed record LoginResponse(DateTime ExpiresAt);

using System.ComponentModel.DataAnnotations;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;

namespace Application.Services;

public sealed class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name?.Trim();
        var email = request.Email?.Trim();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new BusinessRuleException("Name, email, and password are required.");
        if (name.Length > 255 || email.Length > 255 || Encoding.UTF8.GetByteCount(password) > 72)
            throw new BusinessRuleException("Name and email must be at most 255 characters; password must be at most 72 UTF-8 bytes.");
        if (!new EmailAddressAttribute().IsValid(email))
            throw new BusinessRuleException("Email must be valid.");
        if (await userRepository.GetByEmailAsync(email, cancellationToken) is not null)
            throw new BusinessRuleException("A user with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = passwordHasher.Hash(password)
        };
        await userRepository.AddAsync(user, cancellationToken);
        return Map(user);
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    private static UserResponse Map(User user) => new(user.Id, user.Name, user.Email);
}

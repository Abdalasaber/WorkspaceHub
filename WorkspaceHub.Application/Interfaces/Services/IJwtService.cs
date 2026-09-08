using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Interfaces.Services.Auth;

public interface IJwtService
{
    Task<(string Token, DateTime ExpiresAt)> GenerateTokenAsync(
        AppUser user);
}
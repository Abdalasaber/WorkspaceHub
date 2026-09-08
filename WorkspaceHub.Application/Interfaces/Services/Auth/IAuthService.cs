using WorkspaceHub.Application.DTOs.Auth;

namespace WorkspaceHub.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default);

        Task<AuthResponseDto> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default);
    }
}
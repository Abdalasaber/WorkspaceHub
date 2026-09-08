using WorkspaceHub.Application.DTOs.User;

namespace WorkspaceHub.Application.Interfaces.Services
{

    public interface IUserManagementService
    {
        Task<IReadOnlyList<UserResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<UserResponseDto> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default);

        Task ActivateAsync(
            string id,
            CancellationToken cancellationToken = default);

        Task DeactivateAsync(
            string id,
            CancellationToken cancellationToken = default);
    }
}
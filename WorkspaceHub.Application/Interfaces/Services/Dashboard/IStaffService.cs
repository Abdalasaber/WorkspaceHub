using WorkspaceHub.Application.DTOs.Dashboard;
using WorkspaceHub.Application.DTOs.User;

namespace WorkspaceHub.Application.Interfaces.Services
{

    public interface IStaffService
    {
        Task<UserResponseDto> CreateAsync(
            CreateStaffRequest request,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task ActivateAsync(
            string id,
            CancellationToken cancellationToken = default);

        Task DeactivateAsync(
            string id,
            CancellationToken cancellationToken = default);
    }
}
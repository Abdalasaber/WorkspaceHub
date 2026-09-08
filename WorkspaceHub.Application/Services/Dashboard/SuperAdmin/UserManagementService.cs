using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkspaceHub.Application.DTOs.User;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services
{

    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UserManagementService(
            UserManager<AppUser> userManager,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<IReadOnlyList<UserResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var users = _userManager.Users
                    .Include(x => x.Tenant)
                    .OrderBy(x => x.FullName)
                    .ToList();

            var result = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? "None",
                    TenantId = user.TenantId,
                    TenantName = user.Tenant?.Name,
                    IsActive = user.IsActive
                });
            }

            return result;
        }

        public async Task<UserResponseDto> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                throw new NotFoundException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "None",
                TenantId = user.TenantId,
                TenantName = user.Tenant?.Name,
                IsActive = user.IsActive
            };
        }

        public async Task ActivateAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                throw new NotFoundException("User not found.");

            if (user.IsActive)
                throw new ConflictException(
                    "User is already active.");

            user.IsActive = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new ConflictException(
                    string.Join(
                        " ",
                        result.Errors.Select(x => x.Description)));
        }

        public async Task DeactivateAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                throw new NotFoundException("User not found.");

            if (user.Id == _currentUserService.UserId)
                throw new ConflictException(
                    "You cannot deactivate your own account.");

            if (!user.IsActive)
                throw new ConflictException(
                    "User is already inactive.");

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new ConflictException(
                    string.Join(
                        " ",
                        result.Errors.Select(x => x.Description)));
        }

        private void EnsureSuperAdmin()
        {
            if (!_currentUserService.IsSuperAdmin)
            {
                throw new UnauthorizedException(
                    "Only SuperAdmin can manage users.");
            }
        }
    }
}
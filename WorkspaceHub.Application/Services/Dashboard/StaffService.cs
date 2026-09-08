using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkspaceHub.Application.DTOs.Dashboard;
using WorkspaceHub.Application.DTOs.User;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public StaffService(
            UserManager<AppUser> userManager,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<UserResponseDto> CreateAsync(
            CreateStaffRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId =
                _currentUserService.GetRequiredTenantId();

            var existingUser =
                await _userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null)
                throw new ConflictException(
                    "A user with this email already exists.");

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                FullName = request.FullName,
                TenantId = tenantId,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(
                user,
                request.Password);

            if (!result.Succeeded)
                throw new ConflictException(
                    string.Join(
                        " ",
                        result.Errors.Select(x => x.Description)));

            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    "Staff");

            if (!roleResult.Succeeded)
                throw new ConflictException(
                    string.Join(
                        " ",
                        roleResult.Errors.Select(x => x.Description)));

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = "Staff",
                TenantId = user.TenantId,
                IsActive = user.IsActive
            };
        }

        public async Task<IReadOnlyList<UserResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId =
                _currentUserService.GetRequiredTenantId();

            var users = await _userManager.Users
                .Include(x => x.Tenant)
                .Where(x =>
                    x.TenantId == tenantId)
                .ToListAsync(cancellationToken);

            var result = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!roles.Contains("Staff"))
                    continue;

                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    Role = "Staff",
                    TenantId = user.TenantId,
                    TenantName = user.Tenant?.Name,
                    IsActive = user.IsActive
                });
            }

            return result;
        }

        public async Task ActivateAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var tenantId =
                _currentUserService.GetRequiredTenantId();

            var user = await _userManager.FindByIdAsync(id);

            if (user is null ||
                user.TenantId != tenantId)
                throw new NotFoundException(
                    "Staff user not found.");

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Staff"))
                throw new NotFoundException(
                    "Staff user not found.");

            if (user.IsActive)
                throw new ConflictException(
                    "Staff user is already active.");

            user.IsActive = true;

            var result =
                await _userManager.UpdateAsync(user);

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
            var tenantId =
                _currentUserService.GetRequiredTenantId();

            var user = await _userManager.FindByIdAsync(id);

            if (user is null ||
                user.TenantId != tenantId)
                throw new NotFoundException(
                    "Staff user not found.");

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Staff"))
                throw new NotFoundException(
                    "Staff user not found.");

            if (!user.IsActive)
                throw new ConflictException(
                    "Staff user is already inactive.");

            user.IsActive = false;

            var result =
                await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new ConflictException(
                    string.Join(
                        " ",
                        result.Errors.Select(x => x.Description)));
        }
    }
}
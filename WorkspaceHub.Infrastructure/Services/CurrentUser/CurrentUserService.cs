using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;

namespace WorkspaceHub.Infrastructure.Services
{

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        public string? UserId =>
            User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public int? TenantId
        {
            get
            {
                var tenantId = User?.FindFirstValue("tenantId");

                return int.TryParse(tenantId, out var result)
                    ? result
                    : null;
            }
        }

        public string? FullName =>
            User?.FindFirstValue(ClaimTypes.Name);

        public string? Role =>
            User?.FindFirstValue(ClaimTypes.Role);

        public bool IsSuperAdmin =>
            User?.IsInRole("SuperAdmin") ?? false;

        public int GetRequiredTenantId()
        {
            if (IsSuperAdmin)
            {
                throw new ConflictException(
                    "SuperAdmin cannot access tenant resources through this endpoint.");
            }

            if (!TenantId.HasValue)
            {
                throw new UnauthorizedException(
                    "User is not associated with a tenant.");
            }

            return TenantId.Value;
        }
    }
}
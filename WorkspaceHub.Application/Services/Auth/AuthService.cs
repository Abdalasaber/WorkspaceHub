using Microsoft.AspNetCore.Identity;
using WorkspaceHub.Application.DTOs.Auth;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Application.Interfaces.Services.Auth;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public AuthService(
            UserManager<AppUser> userManager,
            IGenericRepository<Tenant> tenantRepository,
            IUnitOfWork unitOfWork,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<RegisterResponseDto> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default)
        {


            var existingUser = await _userManager.FindByEmailAsync(
                request.Email);

            if (existingUser is not null)
                throw new ConflictException(
                    "A user with this email already exists.");

            var slugExists = await _tenantRepository.AnyAsync(
                x => x.Slug == request.TenantSlug,
                cancellationToken);

            if (slugExists)
                throw new ConflictException(
                    "A tenant with this slug already exists.");

            var tenant = new Tenant
            {
                Name = request.TenantName,
                Slug = request.TenantSlug,
                IsActive = false
            };

            await _tenantRepository.AddAsync(
                tenant,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                TenantId = tenant.Id,
                EmailConfirmed = true
            };

            var userResult = await _userManager.CreateAsync(
                user,
                request.Password);

            if (!userResult.Succeeded)
            {
                throw new ConflictException(
                    string.Join(
                        " ",
                        userResult.Errors.Select(x => x.Description)));
            }


            var roleResult = await _userManager.AddToRoleAsync(
                user,
                "Admin");

            if (!roleResult.Succeeded)
            {
                throw new ConflictException(
                    string.Join(
                        " ",
                        roleResult.Errors.Select(x => x.Description)));
            }

            return new RegisterResponseDto
            {
                TenantId = tenant.Id,
                TenantName = tenant.Name,
                AdminEmail = user.Email!,
                Message =
                    "Registration submitted successfully. " +
                    "Your workspace is pending approval."
            };
        }

        public async Task<AuthResponseDto> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new ConflictException(
                    "User account is inactive.");
            }

            var signInResult = await _userManager.CheckPasswordAsync(
                user,
                request.Password);

            //if (signInResult.IsLockedOut)
            //{
            //    throw new ConflictException(
            //        "Account is temporarily locked.");
            //}

            //if (!signInResult.Succeeded)
            //{
            //    throw new UnauthorizedException(
            //        "Invalid email or password.");
            //}

            if (!signInResult)
            {
                throw new UnauthorizedException(
                    "Invalid email or password.");
            }

            if (!user.EmailConfirmed)
            {
                throw new ConflictException(
                    "Email is not confirmed.");
            }

            if (user.TenantId.HasValue)
            {
                var tenant = await _tenantRepository.GetByIdAsync(
                    user.TenantId.Value,
                    cancellationToken: cancellationToken);

                if (tenant is null)
                {
                    throw new UnauthorizedException(
                        "User tenant was not found.");
                }

                if (!tenant.IsActive)
                {
                    throw new UnauthorizedException(
                        "Tenant is inactive.");
                }
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count == 0)
            {
                throw new UnauthorizedException(
                    "User has no assigned role.");
            }

            var (token, expiresAt) =
                await _jwtService.GenerateTokenAsync(user);

            return new AuthResponseDto
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                FullName = user.FullName,
                Role = roles.First(),
                TenantId = user.TenantId
            };
        }

    }
}
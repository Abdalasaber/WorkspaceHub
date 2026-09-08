using AutoMapper;
using WorkspaceHub.Application.DTOs.Tenant;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IGenericRepository<Tenant> _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public TenantService(
            IGenericRepository<Tenant> tenantRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task ApproveAsync(
    int tenantId,
    CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var tenant = await _tenantRepository.GetByIdAsync(
                tenantId,
                cancellationToken: cancellationToken);

            if (tenant is null)
                throw new NotFoundException("Tenant not found.");

            if (tenant.IsActive)
                throw new ConflictException(
                    "Tenant is already active.");

            tenant.IsActive = true;

            _tenantRepository.Update(tenant);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        public async Task<TenantResponseDto> CreateAsync(
            CreateTenantRequest request,
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var slugExists = await _tenantRepository.AnyAsync(
                x => x.Slug == request.Slug,
                cancellationToken);

            if (slugExists)
                throw new ConflictException(
                    "A tenant with this slug already exists.");

            var tenant = _mapper.Map<Tenant>(request);

            tenant.IsActive = true;

            await _tenantRepository.AddAsync(
                tenant,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<TenantResponseDto>(tenant);
        }

        public async Task<IReadOnlyList<TenantResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var tenants = await _tenantRepository.GetAllAsync(
                cancellationToken: cancellationToken);

            return _mapper.Map<IReadOnlyList<TenantResponseDto>>(
                tenants);
        }

        public async Task<TenantResponseDto> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var tenant = await _tenantRepository.GetByIdAsync(
                id,
                cancellationToken: cancellationToken);

            if (tenant is null)
                throw new NotFoundException(
                    "Tenant not found.");

            return _mapper.Map<TenantResponseDto>(tenant);
        }

        public async Task UpdateAsync(
            int id,
            UpdateTenantRequest request,
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var tenant = await _tenantRepository.GetByIdAsync(
                id,
                cancellationToken: cancellationToken);

            if (tenant is null)
                throw new NotFoundException(
                    "Tenant not found.");

            var slugExists = await _tenantRepository.AnyAsync(
                x => x.Slug == request.Slug &&
                     x.Id != id,
                cancellationToken);

            if (slugExists)
                throw new ConflictException(
                    "A tenant with this slug already exists.");

            _mapper.Map(request, tenant);

            _tenantRepository.Update(tenant);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            EnsureSuperAdmin();

            var tenant = await _tenantRepository.GetByIdAsync(
                id,
                cancellationToken: cancellationToken);

            if (tenant is null)
                throw new NotFoundException(
                    "Tenant not found.");

            _tenantRepository.Delete(tenant);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        public async Task<TenantResponseDto> GetCurrentAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId = GetCurrentTenantId();

            var tenant = await _tenantRepository.GetByIdAsync(
                tenantId,
                cancellationToken: cancellationToken);

            if (tenant is null)
                throw new NotFoundException(
                    "Tenant not found.");

            return _mapper.Map<TenantResponseDto>(tenant);
        }

        public async Task UpdateCurrentAsync(
            UpdateCurrentTenantRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = GetCurrentTenantId();

            var tenant = await _tenantRepository.GetByIdAsync(
                tenantId,
                cancellationToken: cancellationToken);

            if (tenant is null)
                throw new NotFoundException(
                    "Tenant not found.");

            var slugExists = await _tenantRepository.AnyAsync(
                x => x.Slug == request.Slug &&
                     x.Id != tenantId,
                cancellationToken);

            if (slugExists)
                throw new ConflictException(
                    "A tenant with this slug already exists.");

            _mapper.Map(request, tenant);

            _tenantRepository.Update(tenant);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        private void EnsureSuperAdmin()
        {
            if (!_currentUserService.IsSuperAdmin)
            {
                throw new UnauthorizedException(
                    "Only SuperAdmin can access this resource.");
            }
        }

        private int GetCurrentTenantId()
        {
            return _currentUserService.GetRequiredTenantId();
        }
    }
}
using AutoMapper;
using WorkspaceHub.Application.DTOs.WorkspaceType;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Domain.Entities;
using WorkspaceTypeHub.Application.Interfaces.Repository;
using WorkspaceTypeHub.Application.Interfaces.Services;

namespace WorkspaceTypeHub.Application.Services
{
    public class WorkspaceTypeService : IWorkspaceTypeService
    {
        private readonly IWorkspaceTypeRepository _workspaceTypeRepository;
        private readonly IGenericRepository<Tenant> _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public WorkspaceTypeService(
            IWorkspaceTypeRepository workspaceTypeRepository,
            IGenericRepository<Tenant> tenantRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService
            )
        {
            _workspaceTypeRepository = workspaceTypeRepository;
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<WorkspaceTypeResponseDto> CreateAsync(
            CreateWorkspaceTypeRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var tenant = await _tenantRepository.GetByIdAsync(
                tenantId,
                cancellationToken: cancellationToken);

            if (tenant is null)
                throw new NotFoundException("Tenant not found.");

            if (!tenant.IsActive)
                throw new ConflictException(
                    "Cannot create workspace type for an inactive tenant.");

            var nameExists = await _workspaceTypeRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.Name == request.Name,
                cancellationToken);

            if (nameExists)
                throw new ConflictException(
                    "A workspace type with this name already exists.");

            var workspaceType = _mapper.Map<WorkspaceType>(request);

            workspaceType.TenantId = tenantId;

            await _workspaceTypeRepository.AddAsync(
                workspaceType,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<WorkspaceTypeResponseDto>(
                workspaceType);
        }
        public async Task UpdateAsync(
            int id,
            UpdateWorkspaceTypeRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspaceType = await _workspaceTypeRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspaceType is null)
                throw new NotFoundException("Workspace type not found.");

            var exists = await _workspaceTypeRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.Name == request.Name &&
                     x.Id != id,
                cancellationToken);

            if (exists)
                throw new ConflictException(
                    "A workspace type with this name already exists.");

            _mapper.Map(request, workspaceType);

            _workspaceTypeRepository.Update(workspaceType);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspaceType = await _workspaceTypeRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspaceType is null)
                throw new NotFoundException("Workspace type not found.");

            _workspaceTypeRepository.Delete(workspaceType);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<WorkspaceTypeResponseDto> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspaceType =
                await _workspaceTypeRepository.GetByIdWithRelationAsync(
                    id,
                    tenantId,
                    cancellationToken);

            if (workspaceType is null)
                throw new NotFoundException(
                    "Workspace type not found.");

            return _mapper.Map<WorkspaceTypeResponseDto>(
                workspaceType);
        }

        public async Task<IReadOnlyList<WorkspaceTypeResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspaceTypes =
                await _workspaceTypeRepository.GetAllWithRelationAsync(
                    tenantId,
                    cancellationToken);

            return _mapper.Map<IReadOnlyList<WorkspaceTypeResponseDto>>(
                workspaceTypes);
        }
    }
}

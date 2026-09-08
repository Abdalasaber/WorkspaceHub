using AutoMapper;
using WorkspaceHub.Application.DTOs.Workspace;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Application.Interfaces.Services.Woekspace;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IGenericRepository<Tenant> _tenantRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IGenericRepository<Branch> _branchRepository;
        private readonly IGenericRepository<WorkspaceType> _workspaceTypeRepository;

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public WorkspaceService(
            IWorkspaceRepository workspaceRepository,
            IGenericRepository<Tenant> tenantRepository,
            IGenericRepository<WorkspaceType> workspaceTypeRepository,
            IGenericRepository<Branch> branchRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService
            )
        {
            _workspaceRepository = workspaceRepository;
            _tenantRepository = tenantRepository;
            _workspaceTypeRepository = workspaceTypeRepository;
            _branchRepository = branchRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<WorkspaceResponseDto> CreateAsync(
            CreateWorkspaceRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var tenant = await _tenantRepository.GetByIdAsync(
                tenantId,
                cancellationToken: cancellationToken);

            if (tenant is null)
                throw new NotFoundException("Tenant not found.");

            var branch = await _branchRepository.GetByIdAsync(
                request.BranchId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (branch is null)
                throw new NotFoundException("Branch not found.");

            var workspaceType = await _workspaceTypeRepository.GetByIdAsync(
                request.WorkspaceTypeId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspaceType is null)
                throw new NotFoundException("Workspace type not found.");

            var nameExists = await _workspaceRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.BranchId == request.BranchId &&
                     x.Name == request.Name,
                cancellationToken);

            if (nameExists)
                throw new ConflictException(
                    "A workspace with this name already exists in this branch.");

            var codeExists = await _workspaceRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.BranchId == request.BranchId &&
                     x.Code == request.Code,
                cancellationToken);

            if (codeExists)
                throw new ConflictException(
                    "A workspace with this code already exists in this branch.");

            var workspace = _mapper.Map<Workspace>(request);

            workspace.TenantId = tenantId;

            await _workspaceRepository.AddAsync(
                workspace,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<WorkspaceResponseDto>(
                workspace);
        }

        public async Task UpdateAsync(
            int id,
            UpdateWorkspaceRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspace = await _workspaceRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspace is null)
                throw new NotFoundException("Workspace not found.");

            var branch = await _branchRepository.GetByIdAsync(
                request.BranchId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (branch is null)
                throw new NotFoundException("Branch not found.");

            var workspaceType = await _workspaceTypeRepository.GetByIdAsync(
                request.WorkspaceTypeId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspaceType is null)
                throw new NotFoundException("Workspace type not found.");

            var nameExists = await _workspaceRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.BranchId == request.BranchId &&
                     x.Name == request.Name &&
                     x.Id != id,
                cancellationToken);

            if (nameExists)
                throw new ConflictException(
                    "A workspace with this name already exists in this branch.");

            var codeExists = await _workspaceRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.BranchId == request.BranchId &&
                     x.Code == request.Code &&
                     x.Id != id,
                cancellationToken);

            if (codeExists)
                throw new ConflictException(
                    "A workspace with this code already exists in this branch.");

            _mapper.Map(request, workspace);

            _workspaceRepository.Update(workspace);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspace = await _workspaceRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspace is null)
                throw new NotFoundException("Workspace not found.");

            _workspaceRepository.Delete(workspace);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<WorkspaceResponseDto> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspace =
                await _workspaceRepository.GetByIdWithRelationAsync(
                    id,
                    tenantId,
                    cancellationToken);

            if (workspace is null)
                throw new NotFoundException("Workspace not found.");

            return _mapper.Map<WorkspaceResponseDto>(
                workspace);
        }

        public async Task<IReadOnlyList<WorkspaceResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspaces =
                await _workspaceRepository.GetAllWithRelationAsync(
                    tenantId,
                    cancellationToken);

            return _mapper.Map<IReadOnlyList<WorkspaceResponseDto>>(
                workspaces);
        }
    }
}
using AutoMapper;
using WorkspaceHub.Application.DTOs.Branch;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services
{

    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IGenericRepository<Tenant> _tenantRepository;
        private readonly IGenericRepository<Workspace> _workspaceRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public BranchService(
            IBranchRepository branchRepository,
            IGenericRepository<Tenant> tenantRepository,
            IGenericRepository<Workspace> workspaceRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _branchRepository = branchRepository;
            _tenantRepository = tenantRepository;
            _workspaceRepository = workspaceRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<BranchResponseDto> CreateAsync(
            CreateBranchRequest request,
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
                    "Cannot create branch for an inactive tenant.");

            var nameExists = await _branchRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.Name == request.Name,
                cancellationToken);

            if (nameExists)
                throw new ConflictException(
                    "A branch with this name already exists.");

            var branch = _mapper.Map<Branch>(request);

            branch.TenantId = tenantId;

            await _branchRepository.AddAsync(
                branch,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<BranchResponseDto>(branch);
        }

        public async Task UpdateAsync(
            int id,
            UpdateBranchRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var branch = await _branchRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (branch is null)
                throw new NotFoundException(
                    "Branch not found.");

            var nameExists = await _branchRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.Name == request.Name &&
                     x.Id != id,
                cancellationToken);

            if (nameExists)
                throw new ConflictException(
                    "A branch with this name already exists.");

            var wasActive = branch.IsActive;
            _mapper.Map(request, branch);

            if (wasActive && !branch.IsActive)
            {
                var workspaces = await _workspaceRepository.GetAllAsync(
                    x => x.TenantId == tenantId && x.BranchId == branch.Id,
                    cancellationToken);

                foreach (var workspace in workspaces)
                {
                    workspace.IsActive = false;
                    workspace.IsAvailable = false;
                    _workspaceRepository.Update(workspace);
                }
            }

            _branchRepository.Update(branch);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var branch = await _branchRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (branch is null)
                throw new NotFoundException(
                    "Branch not found.");

            _branchRepository.Delete(branch);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<BranchResponseDto> GetByIdWithRelationAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var branch = await _branchRepository.GetByIdWithRelationAsync(
                id,
                tenantId,
                cancellationToken);

            if (branch is null)
                throw new NotFoundException(
                    "Branch not found.");

            return _mapper.Map<BranchResponseDto>(branch);
        }

        public async Task<IReadOnlyList<BranchResponseDto>> GetAllWithRelationAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var branches = await _branchRepository.GetAllWithRelationAsync(
                tenantId,
                cancellationToken);

            return _mapper.Map<IReadOnlyList<BranchResponseDto>>(
                branches);
        }
    }
}
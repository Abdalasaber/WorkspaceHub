using AutoMapper;
using WorkspaceHub.Application.DTOs.WorkspacePricing;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Application.Interfaces.Services.WorkspacePricing;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services
{
    public class WorkspacePricingService : IWorkspacePricingService
    {
        private readonly IWorkspacePricingRepository _workspacePricingRepository;
        private readonly IGenericRepository<Workspace> _workspaceRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public WorkspacePricingService(
            IWorkspacePricingRepository workspacePricingRepository,
            IGenericRepository<Workspace> workspaceRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService
            )

        {
            _workspacePricingRepository = workspacePricingRepository;
            _workspaceRepository = workspaceRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<WorkspacePricingResponseDto> CreateAsync(
            CreateWorkspacePricingRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspace = await _workspaceRepository.GetByIdAsync(
                request.WorkspaceId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspace is null)
                throw new NotFoundException("Workspace not found.");

            var exists = await _workspacePricingRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.WorkspaceId == request.WorkspaceId &&
                     x.PricingType == request.PricingType,
                cancellationToken);

            if (exists)
                throw new ConflictException(
                    "The pricing type already exists for this workspace.");

            var workspacePricing = _mapper.Map<WorkspacePricing>(request);

            workspacePricing.TenantId = tenantId;

            workspacePricing.IsActive = true;


            await _workspacePricingRepository.AddAsync(
                workspacePricing,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<WorkspacePricingResponseDto>(
                workspacePricing);
        }

        public async Task UpdateAsync(
            int id,
            UpdateWorkspacePricingRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspacePricing = await _workspacePricingRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspacePricing is null)
                throw new NotFoundException("WorkspacePricing not found.");

            var workspace = await _workspaceRepository.GetByIdAsync(
                request.WorkspaceId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspace is null)
                throw new NotFoundException("Workspace not found.");

            var exists = await _workspacePricingRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.WorkspaceId == request.WorkspaceId &&
                     x.PricingType == request.PricingType &&
                     x.Id != id,
                cancellationToken);

            if (exists)
                throw new ConflictException(
                    "The pricing type already exists for this workspace.");

            _mapper.Map(request, workspacePricing);

            _workspacePricingRepository.Update(workspacePricing);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspacePricing = await _workspacePricingRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (workspacePricing is null)
                throw new NotFoundException("WorkspacePricing not found.");

            _workspacePricingRepository.Delete(workspacePricing);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<WorkspacePricingResponseDto>> GetAllWithRelationAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspacePricings =
                await _workspacePricingRepository.GetAllWithRelationAsync(
                    tenantId,
                    cancellationToken);

            return _mapper.Map<IReadOnlyList<WorkspacePricingResponseDto>>(
                workspacePricings);
        }

        public async Task<WorkspacePricingResponseDto> GetByIdWithRelationAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var workspacePricing =
                await _workspacePricingRepository.GetByIdWithRelationAsync(
                    id,
                    tenantId,
                    cancellationToken);

            if (workspacePricing is null)
                throw new NotFoundException("WorkspacePricing not found.");

            return _mapper.Map<WorkspacePricingResponseDto>(
                workspacePricing);
        }
    }
}
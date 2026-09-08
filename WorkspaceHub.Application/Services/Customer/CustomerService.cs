using AutoMapper;
using WorkspaceHub.Application.DTOs.Customer;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Application.Interfaces.Services.Customer;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services
{

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IGenericRepository<Tenant> _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CustomerService(
            ICustomerRepository customerRepository,
            IGenericRepository<Tenant> tenantRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _customerRepository = customerRepository;
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<CustomerResponseDto> CreateAsync(
            CreateCustomerRequest request,
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
                    "Cannot create customer for an inactive tenant.");

            var nameExists = await _customerRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.FullName == request.FullName,
                cancellationToken);

            if (nameExists)
                throw new ConflictException(
                    "A customer with this full name already exists.");

            var phoneExists = await _customerRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.PhoneNumber == request.PhoneNumber,
                cancellationToken);

            if (phoneExists)
                throw new ConflictException(
                    "A customer with this phone number already exists.");

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExists = await _customerRepository.AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Email == request.Email,
                    cancellationToken);

                if (emailExists)
                    throw new ConflictException(
                        "A customer with this email already exists.");
            }

            var customer = _mapper.Map<Customer>(request);

            customer.TenantId = tenantId;

            await _customerRepository.AddAsync(
                customer,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        public async Task UpdateAsync(
            int id,
            UpdateCustomerRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var customer = await _customerRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (customer is null)
                throw new NotFoundException("Customer not found.");

            var nameExists = await _customerRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.FullName == request.FullName &&
                     x.Id != id,
                cancellationToken);

            if (nameExists)
                throw new ConflictException(
                    "A customer with this full name already exists.");

            var phoneExists = await _customerRepository.AnyAsync(
                x => x.TenantId == tenantId &&
                     x.PhoneNumber == request.PhoneNumber &&
                     x.Id != id,
                cancellationToken);

            if (phoneExists)
                throw new ConflictException(
                    "A customer with this phone number already exists.");

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExists = await _customerRepository.AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.Email == request.Email &&
                         x.Id != id,
                    cancellationToken);

                if (emailExists)
                    throw new ConflictException(
                        "A customer with this email already exists.");
            }

            _mapper.Map(request, customer);

            _customerRepository.Update(customer);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var customer = await _customerRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (customer is null)
                throw new NotFoundException("Customer not found.");

            _customerRepository.Delete(customer);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<IReadOnlyList<CustomerResponseDto>> GetAllWithRelationAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var customers = await _customerRepository.GetAllWithRelationAsync(
                tenantId,
                cancellationToken);

            return _mapper.Map<IReadOnlyList<CustomerResponseDto>>(
                customers);
        }

        public async Task<CustomerResponseDto> GetByIdWithRelationAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var customer = await _customerRepository.GetByIdWithRelationAsync(
                id,
                tenantId,
                cancellationToken);

            if (customer is null)
                throw new NotFoundException("Customer not found.");

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        private int GetTenantId()
        {
            if (_currentUserService.IsSuperAdmin)
                throw new ConflictException(
                    "SuperAdmin cannot access customer resources through this endpoint.");

            if (!_currentUserService.TenantId.HasValue)
                throw new UnauthorizedException(
                    "User is not associated with a tenant.");

            return _currentUserService.TenantId.Value;
        }
    }
}
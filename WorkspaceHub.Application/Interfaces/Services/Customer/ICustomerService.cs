using WorkspaceHub.Application.DTOs.Customer;

namespace WorkspaceHub.Application.Interfaces.Services.Customer
{
    public interface ICustomerService
    {
        Task<CustomerResponseDto> CreateAsync(
            CreateCustomerRequest request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            int id,
            UpdateCustomerRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CustomerResponseDto>> GetAllWithRelationAsync(
            CancellationToken cancellationToken = default);

        Task<CustomerResponseDto> GetByIdWithRelationAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
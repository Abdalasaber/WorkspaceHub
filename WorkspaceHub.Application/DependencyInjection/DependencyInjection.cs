using Microsoft.Extensions.DependencyInjection;
using TenantHub.Application.Mappings;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Application.Interfaces.Services.Customer;
using WorkspaceHub.Application.Interfaces.Services.Dashboard.SuperAdmin;
using WorkspaceHub.Application.Interfaces.Services.Payment;
using WorkspaceHub.Application.Interfaces.Services.Woekspace;
using WorkspaceHub.Application.Interfaces.Services.WorkspacePricing;
using WorkspaceHub.Application.Mappings;
using WorkspaceHub.Application.Services;
using WorkspaceTypeHub.Application.Interfaces.Services;
using WorkspaceTypeHub.Application.Mappings;
using WorkspaceTypeHub.Application.Services;

namespace WorkspaceHub.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ISuperAdminDashboardService, SuperAdminDashboardService>();

            services.AddScoped<IAdminDashboardService, AdminDashboardService>();

            services.AddScoped<IStaffService, StaffService>();

            services.AddScoped<IStaffDashboardService, StaffDashboardService>();

            services.AddScoped<IUserManagementService, UserManagementService>();

            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddAutoMapper(op => op.AddProfile<WprkspaceProfile>());

            services.AddScoped<IWorkspaceTypeService, WorkspaceTypeService>();
            services.AddAutoMapper(op => op.AddProfile<WorkspaceTypeProfile>());

            services.AddScoped<ITenantService, TenantService>();

            services.AddAutoMapper(op => op.AddProfile<TenantProfile>());

            services.AddScoped<IBranchService, BranchService>();
            services.AddAutoMapper(op => op.AddProfile<BranchProfile>());

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddAutoMapper(op => op.AddProfile<CustomerProfile>());

            services.AddScoped<IBookingService, BookingService>();
            services.AddAutoMapper(op => op.AddProfile<BookingProfile>());

            services.AddScoped<IWorkspacePricingService, WorkspacePricingService>();
            services.AddAutoMapper(op => op.AddProfile<WorkspacePricingProfile>());

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddAutoMapper(op => op.AddProfile<PaymentProfile>());

            return services;
        }
    }
}
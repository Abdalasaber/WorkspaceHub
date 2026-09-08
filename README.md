# WorkspaceHub

Multi-tenant workspace booking SaaS backend built with ASP.NET Core and Clean Architecture.

WorkspaceHub is designed for workspace businesses that manage branches, workspaces, workspace types, pricing, customers, bookings, and payments from isolated tenant accounts.

## Highlights

- Multi-tenant architecture with tenant isolation enforced from the authenticated user's JWT claims.
- JWT authentication with ASP.NET Core Identity.
- Role-based authorization with `SuperAdmin`, `Admin`, and `Staff`.
- Public tenant onboarding with SuperAdmin approval before activation.
- Branch, workspace type, workspace, pricing, customer, booking, and payment management.
- Booking conflict prevention and dynamic pricing calculations.
- Partial payments and overpayment tracking.
- Soft delete and audit fields.
- Global exception handling with consistent JSON error responses.
- SuperAdmin platform dashboard and user management.
- Tenant Admin dashboard and Staff management.
- Staff operational home.
- Swagger/OpenAPI for API exploration.

## Architecture

WorkspaceHub follows a practical, layer-based Clean Architecture approach:

```text
WorkspaceHub.Api
    ↓
WorkspaceHub.Application
    ↓
WorkspaceHub.Domain

WorkspaceHub.Infrastructure
    ├── EF Core / SQL Server
    ├── ASP.NET Core Identity
    ├── JWT
    └── Repositories / Persistence
```

### Projects

```text
WorkspaceHub/
├── WorkspaceHub.Api/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings*.json
│
├── WorkspaceHub.Application/
│   ├── DTOs/
│   ├── Exceptions/
│   ├── Interfaces/
│   ├── Mappings/
│   ├── Services/
│   ├── Settings/
│   └── DependencyInjection/
│
├── WorkspaceHub.Domain/
│   ├── Common/Base/
│   ├── Entities/
│   └── Enums/
│
├── WorkspaceHub.Infrastructure/
│   ├── Extensions/
│   ├── Migrations/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Context/
│   │   └── Repositories/
│   └── Services/
│
├── docs/
│   └── API_DOCUMENTATION.md
│
├── .gitignore
└── WorkspaceHub.sln
```

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- ASP.NET Core Identity
- JWT Bearer Authentication
- AutoMapper
- Swashbuckle / Swagger

## Roles

### SuperAdmin

Platform-level administrator. A SuperAdmin is not attached to a tenant and is responsible for platform administration:

- View platform dashboard.
- Review and approve tenant onboarding requests.
- View/manage platform users.
- Activate/deactivate user accounts.
- Manage tenants through SuperAdmin-only tenant endpoints.

### Admin

Tenant-level administrator. Each Admin belongs to one tenant and can manage the tenant's operational data and staff.

### Staff

Operational tenant user. Staff can work with day-to-day customers, bookings, payments, and read operational workspace information according to endpoint permissions.

## Onboarding Flow

```text
Public Register
      ↓
Create inactive Tenant
      ↓
Create first Tenant Admin
      ↓
SuperAdmin approval
      ↓
Tenant becomes active
      ↓
Admin can log in
      ↓
Admin creates Staff users
```

The public registration endpoint never accepts a role from the client. The first user of a tenant is always created as `Admin`.

## Authorization Model

| Area | SuperAdmin | Admin | Staff |
|---|---:|---:|---:|
| SuperAdmin Dashboard | ✅ | ❌ | ❌ |
| SuperAdmin User Management | ✅ | ❌ | ❌ |
| Tenant Administration | ✅* | ✅** | ❌ |
| Branch Read | ❌* | ✅ | ✅ |
| Branch Write/Delete | ❌* | ✅ | ❌ |
| Workspace Type Read | ❌* | ✅ | ✅ |
| Workspace Type Write/Delete | ❌* | ✅ | ❌ |
| Workspace Read | ❌* | ✅ | ✅ |
| Workspace Write/Delete | ❌* | ✅ | ❌ |
| Pricing Read | ❌* | ✅ | ✅ |
| Pricing Write/Delete | ❌* | ✅ | ❌ |
| Customer Read/Create/Update | ❌* | ✅ | ✅ |
| Customer Delete | ❌* | ✅ | ❌ |
| Booking Read/Create/Update | ❌* | ✅ | ✅ |
| Booking Delete | ❌* | ✅ | ❌ |
| Payment Read/Create | ❌* | ✅ | ✅ |
| Payment Delete | ❌* | ✅ | ❌ |
| Admin Dashboard | ❌ | ✅ | ❌ |
| Staff Management | ❌ | ✅ | ❌ |
| Staff Home | ❌ | ❌ | ✅ |

`*` SuperAdmin has separate platform endpoints rather than tenant-scoped operational endpoints.

`**` Admin can update only its current tenant through `/api/Tenants/me`; tenant activation/approval is SuperAdmin responsibility.

## Tenant Isolation

Tenant-owned resources derive the tenant ID from the authenticated user's JWT claim through `ICurrentUserService.GetRequiredTenantId()`.

Clients must not be trusted to provide a tenant ID for tenant-scoped operations.

This means a tenant user cannot access another tenant's branches, workspaces, customers, bookings, or payments simply by changing an ID in the request.

## Payment Model

Payments intentionally track both the money received and the amount applied to the booking:

```text
Amount         = actual money received
AppliedAmount  = amount applied to the booking
Overpayment    = Amount - AppliedAmount
```

Example:

```text
Booking remaining = 170
Customer pays      = 200
AppliedAmount      = 170
Overpayment        = 30
```

The payment summary exposes both `TotalReceived` and `TotalPaid` so financial review remains accurate.

## Important Business Rules

- Tenant slug is unique among non-deleted tenants.
- Branch name is unique per tenant.
- Workspace type name is unique per tenant.
- Workspace name and code are unique per branch among non-deleted workspaces.
- Customer name and phone are unique per tenant; email is unique per tenant when provided.
- Workspace pricing is unique per workspace and pricing type among non-deleted records.
- An inactive tenant cannot perform normal tenant operations.
- An inactive user cannot log in.
- Five failed login attempts cause a temporary Identity lockout according to the configured policy.
- A booking cannot start in the past.
- A booking cannot overlap another non-cancelled booking for the same workspace.
- Cancelled bookings do not block the workspace.
- Booking price is taken from configured workspace pricing; the client does not provide `UnitPrice`.
- New payments cannot be added to a cancelled booking.
- A fully paid booking cannot accept another payment.
- Payment overages are retained as `Amount - AppliedAmount` and do not inflate booking revenue.
- A booking with existing payments cannot change financial/relationship fields such as workspace, customer, pricing type, time, discount, or tax.

## Configuration

Do not commit real secrets to GitHub.

### `appsettings.json`

The repository contains safe placeholders for local configuration.

### Local development with User Secrets

From the API project directory:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR-CONNECTION-STRING"
dotnet user-secrets set "JwtSettings:Key" "YOUR-LONG-RANDOM-JWT-KEY"
```

Optional frontend CORS origin:

```bash
dotnet user-secrets set "Frontend:Origin" "https://localhost:4200"
```

### Production

Use environment variables or a managed secret store for database credentials, JWT signing keys, API keys, and other secrets.

## Database

EF Core migrations are included in the repository.

Update the database with:

```bash
dotnet ef database update \
  --project WorkspaceHub.Infrastructure \
  --startup-project WorkspaceHub.Api
```

Create a new migration with:

```bash
dotnet ef migrations add MigrationName \
  --project WorkspaceHub.Infrastructure \
  --startup-project WorkspaceHub.Api
```

## Run Locally

1. Clone the repository.
2. Restore dependencies:

```bash
dotnet restore
```

3. Configure User Secrets.
4. Apply EF Core migrations.
5. Start the API:

```bash
dotnet run --project WorkspaceHub.Api
```

6. Open Swagger in Development.

Swagger is configured for the Development environment only.

## API Documentation

See the complete endpoint reference:

[API Documentation](docs/API_DOCUMENTATION.md)

## Project Status

WorkspaceHub is a tested portfolio-ready backend implementation for a multi-tenant workspace booking SaaS. The core authentication, authorization, tenant isolation, operational modules, payments, dashboards, and exception handling are implemented.

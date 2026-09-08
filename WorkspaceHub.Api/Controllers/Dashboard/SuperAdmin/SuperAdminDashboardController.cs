using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.Interfaces.Services.Dashboard.SuperAdmin;

namespace WorkspaceHub.Api.Controllers.SuperAdmin;

[Route("api/superadmin/dashboard")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminDashboardController : ControllerBase
{
    private readonly ISuperAdminDashboardService _service;

    public SuperAdminDashboardController(
        ISuperAdminDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(
            cancellationToken);

        return Ok(result);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.Interfaces.Services;

namespace WorkspaceHub.Api.Controllers.Staff;

[Route("api/staff/home")]
[ApiController]
[Authorize(Roles = "Staff")]
public class StaffHomeController : ControllerBase
{
    private readonly IStaffDashboardService _service;

    public StaffHomeController(
        IStaffDashboardService service)
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
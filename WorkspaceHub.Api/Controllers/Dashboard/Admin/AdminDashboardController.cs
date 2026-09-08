using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.Interfaces.Services;

namespace WorkspaceHub.Api.Controllers.Admin;

[Route("api/admin/dashboard")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _service;

    public AdminDashboardController(
        IAdminDashboardService service)
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
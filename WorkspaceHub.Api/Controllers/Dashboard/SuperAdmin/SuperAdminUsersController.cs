using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.Interfaces.Services;

namespace WorkspaceHub.Api.Controllers.SuperAdmin;

[Route("api/superadmin/users")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminUsersController : ControllerBase
{
    private readonly IUserManagementService _service;

    public SuperAdminUsersController(
        IUserManagementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(
            id,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id}/activate")]
    public async Task<IActionResult> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        await _service.ActivateAsync(
            id,
            cancellationToken);

        return NoContent();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        await _service.DeactivateAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}
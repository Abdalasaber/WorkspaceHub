using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.DTOs.Dashboard;
using WorkspaceHub.Application.Interfaces.Services;

namespace WorkspaceHub.Api.Controllers.Admin;

[Route("api/admin/staff")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminStaffController : ControllerBase
{
    private readonly IStaffService _service;

    public AdminStaffController(
        IStaffService service)
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

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateStaffRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.DTOs.Tenant;
using WorkspaceHub.Application.Interfaces.Services;

namespace WorkspaceHub.Api.Controllers.Tenant
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _service;

        public TenantsController(ITenantService service)
        {
            _service = service;
        }

        [HttpPost("{id:int}/approve")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Approve(
            int id,
            CancellationToken cancellationToken)
        {
            await _service.ApproveAsync(
                id,
                cancellationToken);

            return NoContent();
        }

        [HttpGet("me")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCurrent(
            CancellationToken cancellationToken)
        {
            var result = await _service.GetCurrentAsync(
                cancellationToken);

            return Ok(result);
        }

        [HttpPut("me")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCurrent(
            [FromBody] UpdateCurrentTenantRequest request,
            CancellationToken cancellationToken)
        {
            await _service.UpdateCurrentAsync(
                request,
                cancellationToken);

            return NoContent();
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(
                id,
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateTenantRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTenantRequest request,
            CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(
                id,
                request,
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(
                id,
                cancellationToken);

            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.DTOs.WorkspacePricing;
using WorkspaceHub.Application.Interfaces.Services.WorkspacePricing;

namespace WorkspaceHub.Api.Controllers.WorkspacePricing
{

    [Route("api/[controller]")]
    [ApiController]
    public class WorkspacePricingsController : ControllerBase
    {
        private readonly IWorkspacePricingService _service;

        public WorkspacePricingsController(IWorkspacePricingService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            var result = await _service.GetAllWithRelationAsync(
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdWithRelationAsync(
                id,
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateWorkspacePricingRequest request,
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateWorkspacePricingRequest request,
            CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(
                id,
                request,
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
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
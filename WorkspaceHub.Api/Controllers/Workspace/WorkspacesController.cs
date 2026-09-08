using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.DTOs.Workspace;
using WorkspaceHub.Application.Interfaces.Services.Woekspace;

namespace WorkspaceHub.Api.Controllers.Workspace
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _service;

        public WorkspacesController(IWorkspaceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            var res = await _service.GetAllAsync(
                cancellationToken);

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var res = await _service.GetByIdAsync(
                id,
                cancellationToken);

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkspace(
            [FromBody] CreateWorkspaceRequest request,
            CancellationToken cancellationToken)
        {
            var res = await _service.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = res.Id },
                res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateWorkspaceRequest request,
            CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(
                id,
                request,
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
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
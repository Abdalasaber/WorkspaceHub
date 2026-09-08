using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WorkspaceHub.Application.DTOs.WorkspaceType;
using WorkspaceTypeHub.Application.Interfaces.Services;

namespace WorkspaceTypeHub.Api.Controllers.WorkspaceTypeType
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspaceTypesController : ControllerBase
    {
        private readonly IWorkspaceTypeService _service;

        public WorkspaceTypesController(IWorkspaceTypeService service)
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

        [HttpGet("{id:int}")]
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
        public async Task<IActionResult> CreateWorkspaceType(
            [FromBody] CreateWorkspaceTypeRequest request,
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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateWorkspaceTypeRequest request,
            CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(
                id,
                request,
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
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
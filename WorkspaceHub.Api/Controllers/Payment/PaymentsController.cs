using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkspaceHub.Application.DTOs.Payment;
using WorkspaceHub.Application.Interfaces.Services.Payment;

namespace WorkspaceHub.Api.Controllers.Payment
{

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentsController(IPaymentService service)
        {
            _service = service;
        }

        [HttpGet("booking/{bookingId:int}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetByBooking(
            int bookingId,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetByBookingAsync(
                bookingId,
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(
            [FromBody] CreatePaymentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(
                request,
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _service.DeleteAsync(
                id,
                cancellationToken);

            return Ok(result);
        }
    }
}
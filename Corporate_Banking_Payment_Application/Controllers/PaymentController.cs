using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a list of all payments.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _service.GetAllPayments();
            return Ok(payments);
        }

        /// <summary>
        /// Retrieves a specific payment by ID.
        /// </summary>
        /// <param name="id">The ID of the payment.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _service.GetPaymentById(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        /// <summary>
        /// Retrieves payments associated with a specific client.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetPaymentsByClientId(int clientId)
        {
            var payments = await _service.GetPaymentsByClientId(clientId);
            // Returns 200 OK even if the list is empty, which is appropriate for a collection lookup.
            return Ok(payments);
        }

        /// <summary>
        /// Retrieves payments associated with a specific beneficiary.
        /// </summary>
        /// <param name="beneficiaryId">The ID of the beneficiary.</param>
        [HttpGet("beneficiary/{beneficiaryId}")]
        public async Task<IActionResult> GetPaymentsByBeneficiaryId(int beneficiaryId)
        {
            var payments = await _service.GetPaymentsByBeneficiaryId(beneficiaryId);
            return Ok(payments);
        }

        /// <summary>
        /// Retrieves payments based on their status (PENDING, APPROVED, REJECTED).
        /// </summary>
        /// <param name="status">The payment status.</param>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetPaymentsByStatus(Status status)
        {
            var payments = await _service.GetPaymentsByStatus(status);
            return Ok(payments);
        }

        /// <summary>
        /// Creates a new payment request.
        /// </summary>
        /// <param name="dto">The data transfer object for creating a payment.</param>
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _service.CreatePayment(dto);
                // Return a 201 Created response with a location header
                return CreatedAtAction(nameof(GetPaymentById), new { id = created.PaymentId }, created);
            }
            catch (Exception ex)
            {
                // Catches exceptions from service layer (e.g., client/beneficiary not found)
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the status and rejection reason of an existing payment.
        /// </summary>
        /// <param name="id">The ID of the payment to update.</param>
        /// <param name="dto">The data transfer object containing the status update.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdatePayment(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        /// <summary>
        /// Deletes a specific payment record.
        /// </summary>
        /// <param name="id">The ID of the payment to delete.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var deleted = await _service.DeletePayment(id);
            if (!deleted)
            {
                // This could mean the payment wasn't found (404) or couldn't be deleted 
                // because it wasn't PENDING (400).
                var exists = await _service.GetPaymentById(id);
                if (exists == null) return NotFound();

                return BadRequest(new { message = "Only PENDING payments can be deleted." });
            }

            // 204 No Content is standard for successful deletion
            return NoContent();
        }
    }
}

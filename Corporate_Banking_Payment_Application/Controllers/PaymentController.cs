using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
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

        ///// Retrieves a list of all payments.
        //[HttpGet]
        //public async Task<IActionResult> GetAllPayments()
        //{
        //    var payments = await _service.GetAllPayments();
        //    return Ok(payments);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllPayments(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] SortOrder? sortOrder = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var payments = await _service.GetAllPayments(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            return Ok(payments);
        }

        /// Retrieves a specific payment by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _service.GetPaymentById(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }


        /// Retrieves payments associated with a specific client
        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetPaymentsByClientId(int clientId)
        {
            var payments = await _service.GetPaymentsByClientId(clientId);
            // Returns 200 OK even if the list is empty, which is appropriate for a collection lookup.
            return Ok(payments);
        }


        /// Retrieves payments associated with a specific beneficiary.
        [HttpGet("beneficiary/{beneficiaryId}")]
        public async Task<IActionResult> GetPaymentsByBeneficiaryId(int beneficiaryId)
        {
            var payments = await _service.GetPaymentsByBeneficiaryId(beneficiaryId);
            return Ok(payments);
        }

  
        /// Retrieves payments based on their status (PENDING, APPROVED, REJECTED).
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetPaymentsByStatus(Status status)
        {
            var payments = await _service.GetPaymentsByStatus(status);
            return Ok(payments);
        }


        /// Creates a new payment request
        [Authorize(Roles = "CLIENTUSER")]
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


        /// Updates the status and rejection reason of an existing payment.
        [Authorize(Roles = "BANKUSER")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdatePayment(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }


        /// Deletes a specific payment record
        [Authorize(Roles = "SUPERADMIN,BANKUSER")]
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

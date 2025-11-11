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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _service.GetPaymentById(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }



        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetPaymentsByClientId(int clientId)
        {
            var payments = await _service.GetPaymentsByClientId(clientId);

            return Ok(payments);
        }



        [HttpGet("beneficiary/{beneficiaryId}")]
        public async Task<IActionResult> GetPaymentsByBeneficiaryId(int beneficiaryId)
        {
            var payments = await _service.GetPaymentsByBeneficiaryId(beneficiaryId);
            return Ok(payments);
        }



        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetPaymentsByStatus(Status status)
        {
            var payments = await _service.GetPaymentsByStatus(status);
            return Ok(payments);
        }



        [Authorize(Roles = "CLIENTUSER")]
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _service.CreatePayment(dto);

                return CreatedAtAction(nameof(GetPaymentById), new { id = created.PaymentId }, created);
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }



        [Authorize(Roles = "BANKUSER")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdatePayment(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }



        [Authorize(Roles = "SUPERADMIN,BANKUSER,CLIENTUSER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var deleted = await _service.DeletePayment(id);
            if (!deleted)
            {

                var exists = await _service.GetPaymentById(id);
                if (exists == null) return NotFound();

                return BadRequest(new { message = "Only PENDING payments can be deleted." });
            }


            return NoContent();
        }


    }
}
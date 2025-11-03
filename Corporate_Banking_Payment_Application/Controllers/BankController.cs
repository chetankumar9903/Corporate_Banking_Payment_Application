using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly IBankService _service;

        public BankController(IBankService service)
        {
            _service = service;
        }
        ////[Authorize(Roles = "SUPERADMIN")]
        //[HttpGet]
        //public async Task<IActionResult> GetAllBank()
        //{
        //    var banks = await _service.GetAllBank();
        //    return Ok(banks);
        //}

        //[Authorize(Roles = "SUPERADMIN")]
        // MODIFIED: This endpoint now accepts query parameters
        [HttpGet]
        public async Task<IActionResult> GetAllBank(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] SortOrder? sortOrder = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var banks = await _service.GetAllBank(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            return Ok(banks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankById(int id)
        {
            var bank = await _service.GetBankById(id);
            if (bank == null) return NotFound();
            return Ok(bank);
        }

        //[Authorize(Roles = "SUPERADMIN")]
        [HttpPost]
        public async Task<IActionResult> CreateBank([FromBody] CreateBankDto dto)
        {
            if (!ModelState.IsValid)

                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateBank(dto);
                return CreatedAtAction(nameof(GetBankById), new { id = created.BankId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize(Roles = "SUPERADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] UpdateBankDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateBank(id, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        //[Authorize(Roles = "SUPERADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteBank(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}

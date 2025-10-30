using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeneficiaryController : ControllerBase
    {
        private readonly IBeneficiaryService _service;

        public BeneficiaryController(IBeneficiaryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var beneficiaries = await _service.GetAllBeneficiaries();
            return Ok(beneficiaries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var beneficiary = await _service.GetBeneficiaryById(id);
            if (beneficiary == null) return NotFound();
            return Ok(beneficiary);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClientId(int clientId)
        {
            try
            {
                var beneficiaries = await _service.GetBeneficiariesByClientId(clientId);
                if (!beneficiaries.Any())
                    return NotFound(new { message = "No beneficiaries found for this client." });

                return Ok(beneficiaries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBeneficiaryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateBeneficiary(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.BeneficiaryId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBeneficiaryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateBeneficiary(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteBeneficiary(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}

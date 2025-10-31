using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryDisbursementController : ControllerBase
    {
        private readonly ISalaryDisbursementService _service;

        public SalaryDisbursementController(ISalaryDisbursementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAll();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClientId(int clientId)
        {
            var result = await _service.GetByClientId(clientId);
            return Ok(result);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployeeId(int employeeId)
        {
            var result = await _service.GetByEmployeeId(employeeId);
            return Ok(result);
        }
        [Authorize(Roles = "CLIENTUSER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSalaryDisbursementDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //var created = await _service.Create(dto);
            //return CreatedAtAction(nameof(GetById), new { id = created.SalaryDisbursementId }, created);

            try
            {
                var created = await _service.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.SalaryDisbursementId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "CLIENTUSER")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSalaryDisbursementDto dto)
        {
            //var updated = await _service.Update(id, dto);
            //return Ok(updated);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.Update(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        [Authorize(Roles = "CLIENTUSER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}

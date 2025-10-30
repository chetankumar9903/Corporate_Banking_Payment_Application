using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllCustomers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetCustomerById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //var result = await _service.CreateCustomer(dto);
            //return CreatedAtAction(nameof(GetById), new { id = result.CustomerId }, result);

            try
            {
                var result = await _service.CreateCustomer(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.CustomerId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //var result = await _service.UpdateCustomer(id, dto);
            //return Ok(result);

            try
            {
                var result = await _service.UpdateStatus(id, dto.VerificationStatus);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteCustomer(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] Status newStatus)
        {
            try
            {
                var result = await _service.UpdateStatus(id, newStatus);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[HttpGet("byUser/{userId}")]
        //public async Task<IActionResult> GetByUserId(int userId)
        //{
        //    var result = await _service.GetCustomerByUserId(userId);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}
    }
}

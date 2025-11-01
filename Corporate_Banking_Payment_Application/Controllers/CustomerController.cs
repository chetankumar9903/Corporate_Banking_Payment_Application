using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
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

        //[HttpGet]
        ////[Authorize(Roles = "BANKUSER")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var result = await _service.GetAllCustomers();
        //    return Ok(result);
        //}

        [HttpGet]
        //[Authorize(Roles = "BANKUSER")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] SortOrder? sortOrder = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllCustomers(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetCustomerById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        //[Authorize(Roles = "BANKUSER")]
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

        //[Authorize(Roles = "BANKUSER")]
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

        //[Authorize(Roles = "BANKUSER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteCustomer(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        //[Authorize(Roles = "BANKUSER")]
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

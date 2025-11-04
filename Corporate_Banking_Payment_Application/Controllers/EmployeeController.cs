using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        ///// Retrieves a list of all employees.
        //[HttpGet]
        //public async Task<IActionResult> GetAllEmployees()
        //{
        //    var employees = await _service.GetAllEmployees();
        //    return Ok(employees);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] SortOrder? sortOrder = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var employees = await _service.GetAllEmployees(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            return Ok(employees);
        }

        /// Retrieves a specific employee by ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _service.GetEmployeeById(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }


        /// Creates a new employee record.
        [Authorize(Roles = "CLIENTUSER")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateEmployee(dto);
                // Return a 201 Created response with a location header pointing to the new resource
                return CreatedAtAction(nameof(GetEmployeeById), new { id = created.EmployeeId }, created);
            }
            catch (Exception ex)
            {
                // This handles potential business logic exceptions (e.g., duplicate employee code)
                return BadRequest(new { message = ex.Message });
            }
        }

        
        /// Updates an existing employee record.
        [Authorize(Roles = "CLIENTUSER")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateEmployee(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

       
        /// Deletes a specific employee record
        [Authorize(Roles = "CLIENTUSER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted = await _service.DeleteEmployee(id);
            if (!deleted) return NotFound();

            // 204 No Content is standard for successful deletion
            return NoContent();
        }

        [HttpGet("byclient/{clientId}")]
        public async Task<IActionResult> GetEmployeesByClientId(int clientId)
        {
            try
            {
                var employees = await _service.GetEmployeesByClientId(clientId);
                if (!employees.Any())
                    return NotFound(new { message = "No employees found for this client." });

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

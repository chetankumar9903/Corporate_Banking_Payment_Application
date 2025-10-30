using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Services.IService;
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

        /// <summary>
        /// Retrieves a list of all employees.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _service.GetAllEmployees();
            return Ok(employees);
        }

        /// <summary>
        /// Retrieves a specific employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _service.GetEmployeeById(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        /// <summary>
        /// Creates a new employee record.
        /// </summary>
        /// <param name="dto">The data transfer object for creating an employee.</param>
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

        /// <summary>
        /// Updates an existing employee record.
        /// </summary>
        /// <param name="id">The ID of the employee to update.</param>
        /// <param name="dto">The data transfer object for updating an employee.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateEmployee(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        /// <summary>
        /// Deletes a specific employee record.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
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

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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _service.GetEmployeeById(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }



        [Authorize(Roles = "CLIENTUSER")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateEmployee(dto);

                return CreatedAtAction(nameof(GetEmployeeById), new { id = created.EmployeeId }, created);
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }



        [Authorize(Roles = "CLIENTUSER")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateEmployee(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }


        [Authorize(Roles = "CLIENTUSER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {

            try
            {
                await _service.DeleteEmployee(id);
                return Ok(new { Message = "Employee deactivated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
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




        [Authorize(Roles = "CLIENTUSER")]
        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadEmployeesCsv(IFormFile file, [FromQuery] int clientId)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "CSV file is required." });


            if (!file.FileName.EndsWith(".csv"))
                return BadRequest(new { message = "Only CSV files are allowed." });

            try
            {
                var result = await _service.ProcessEmployeeCsv(file, clientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

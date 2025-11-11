using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Mvc;


namespace Corporate_Banking_Payment_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    var users = await _service.GetAllUsers();
        //    return Ok(users);
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] SortOrder? sortOrder = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var users = await _service.GetAllUsers(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _service.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateUser(dto);
                return CreatedAtAction(nameof(GetUserById), new { id = created.UserId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateUser(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _service.DeleteUser(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("available-client-users")]
        public async Task<IActionResult> GetAvailableClientUsers()
        {
            var users = await _service.GetAvailableClientUsers();
            return Ok(users);
        }



        [HttpGet("bankusers")]
        public async Task<IActionResult> GetBankUsers()
        {
            var users = await _service.GetUnassignedBankUsersAsync();
            var response = users.Select(u => new { u.UserId, u.UserName });
            return Ok(response);
        }


    }
}

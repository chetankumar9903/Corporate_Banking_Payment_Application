using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientController(IClientService service)
        {
            _service = service;
        }

        ////[Authorize(Roles = "BANKUSER")]
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var clients = await _service.GetAllClients();
        //    return Ok(clients);
        //}

        [Authorize(Roles = "BANKUSER")]
        // MODIFIED: This endpoint now accepts query parameters
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] SortOrder? sortOrder = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var clients = await _service.GetAllClients(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            return Ok(clients);
        }

        [Authorize(Roles = "BANKUSER")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _service.GetClientById(id);
            if (client == null) return NotFound();
            return Ok(client);
        }

        [Authorize(Roles = "BANKUSER")]
        // Get client by CustomerId
        [HttpGet("byCustomer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var client = await _service.GetClientByCustomerId(customerId);
            if (client == null) return NotFound();
            return Ok(client);
        }



        // Create new client (only if customer approved)
        [Authorize(Roles = "BANKUSER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.CreateClient(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.ClientId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "BANKUSER")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.UpdateClient(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "BANKUSER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteClient(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/balance")]
        public async Task<IActionResult> UpdateBalance(int id, [FromBody] UpdateClientBalanceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdateClientBalance(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Catches errors like "Insufficient funds"
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{clientId}/balance")]
        public async Task<IActionResult> GetBalance(int clientId)
        {
            var client = await _service.GetClientById(clientId);
            if (client == null) return NotFound();
            return Ok(new { balance = client.Balance });
        }

    }
}

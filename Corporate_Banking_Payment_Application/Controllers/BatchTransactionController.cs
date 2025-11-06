using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatchTransactionController : ControllerBase
    {
        private readonly IBatchTransactionService _service;

        public BatchTransactionController(IBatchTransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllBatches();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetBatchById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetByClientId(int clientId)
            => Ok(await _service.GetByClientId(clientId));

        [Authorize(Roles = "CLIENTUSER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBatchTransactionDto dto)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            //var created = await _service.CreateBatch(dto);
            //return CreatedAtAction(nameof(GetById), new { id = created.BatchId }, created);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _service.CreateBatch(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.BatchId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize(Roles = "CLIENTUSER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteBatch(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "CLIENTUSER")]
        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadBatchCsv(IFormFile file, [FromQuery] int clientId)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "CSV file is required." });

            if (!file.FileName.EndsWith(".csv"))
                return BadRequest(new { message = "Only CSV files are allowed." });

            try
            {
                var result = await _service.ProcessBatchCsv(file, clientId);
                return Ok(result); // { created, skipped }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

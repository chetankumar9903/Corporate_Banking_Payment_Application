using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.Controllers
{

    public class DocumentUploadRequest
    {

        [Required]
        public int CustomerId { get; set; }

        [Required, MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;


        [Required]
        public IFormFile? File { get; set; }
    }


    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _service;

        public DocumentController(IDocumentService service)
        {
            _service = service;
        }



        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "BANKUSER")]
        public async Task<IActionResult> GetAllDocuments(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] SortOrder? sortOrder = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var documents = await _service.GetAllDocuments(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            return Ok(documents);
        }


        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentsByCustomerId(int customerId)
        {
            var documents = await _service.GetDocumentsByCustomerId(customerId);
            return Ok(documents);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var document = await _service.GetDocumentById(id);
            if (document == null) return NotFound();
            return Ok(document);
        }


        [Authorize(Roles = "BANKUSER")]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadRequest request)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);


            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("File content is missing or empty.");
            }


            var createDto = new CreateDocumentDto
            {
                CustomerId = request.CustomerId,
                DocumentType = request.DocumentType
            };

            try
            {

                var created = await _service.UploadDocument(request.File, createDto);


                return CreatedAtAction(nameof(GetDocumentById), new { id = created.DocumentId }, created);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Document processing failed.", details = ex.Message });
            }
        }


        [Authorize(Roles = "BANKUSER")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocumentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateDocument(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }



        [Authorize(Roles = "BANKUSER")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            try
            {
                var deleted = await _service.DeleteDocument(id);
                if (!deleted) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Deletion failed. File may still exist in Cloudinary.", details = ex.Message });
            }
        }



    }
}

using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.Controllers
{
    // *** NEW MODEL TO RESOLVE SWAGGER ERROR ***
    // This model combines the IFormFile and the metadata DTO fields 
    // into a single object for the [FromForm] binding to work correctly with Swashbuckle.
    public class DocumentUploadRequest
    {
        // Metadata fields from CreateDocumentDto
        [Required]
        public int CustomerId { get; set; }

        [Required, MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        // The file itself
        [Required]
        public IFormFile? File { get; set; }
    }
    // ******************************************

    [ApiController]
    [Route("api/[controller]")] // Routes to /api/Document
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _service;

        public DocumentController(IDocumentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all document metadata for the application. (Use sparingly, primarily for admin/auditing).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "BANKUSER")]
        public async Task<IActionResult> GetAllDocuments()
        {
            var documents = await _service.GetAllDocuments();
            return Ok(documents);
        }

        /// <summary>
        /// Retrieves all documents associated with a specific Customer ID.
        /// </summary>
        /// <param name="customerId">The ID of the Customer whose documents are to be retrieved.</param>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentsByCustomerId(int customerId)
        {
            var documents = await _service.GetDocumentsByCustomerId(customerId);
            return Ok(documents);
        }

        /// <summary>
        /// Retrieves metadata for a specific document by its ID.
        /// </summary>
        /// <param name="id">The ID of the document.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var document = await _service.GetDocumentById(id);
            if (document == null) return NotFound();
            return Ok(document);
        }

        /// <summary>
        /// Uploads a new document to Cloudinary and saves its reference to the database.
        /// </summary>
        /// <param name="request">Contains the file content (IFormFile) and metadata (CustomerId, DocumentType).</param>
        [Authorize(Roles = "BANKUSER")]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Essential for Swagger/Postman to recognize file upload
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // FIX: Combine IFormFile and DTO into a single class for Swashbuckle compatibility
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadRequest request)
        {
            // ModelState validation is handled on the DocumentUploadRequest model
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // File presence check (now using the request model)
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("File content is missing or empty.");
            }

            // Map fields to the service's expected DTO structure for consistency
            var createDto = new CreateDocumentDto
            {
                CustomerId = request.CustomerId,
                DocumentType = request.DocumentType
            };

            try
            {
                // Pass the IFormFile and the mapped DTO to the service
                var created = await _service.UploadDocument(request.File, createDto);

                // 201 Created response points to the single document resource
                return CreatedAtAction(nameof(GetDocumentById), new { id = created.DocumentId }, created);
            }
            catch (ArgumentException ex)
            {
                // Handles ArgumentException from the service (e.g., file validation failure)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handles Cloudinary or database errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Document processing failed.", details = ex.Message });
            }
        }

        /// <summary>
        /// Updates a document record (e.g., changing its status).
        /// </summary>
        /// <param name="id">The ID of the document to update.</param>
        /// <param name="dto">The update DTO (e.g., IsActive status).</param>
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


        /// <summary>
        /// Deletes the document record and the corresponding file from Cloudinary.
        /// </summary>
        /// <param name="id">The ID of the document to delete.</param>
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
                if (!deleted) return NotFound(); // Returns 404 if ID not found

                return NoContent(); // 204 No Content on successful deletion
            }
            catch (Exception ex)
            {
                // Catches the specific exception thrown by the service if Cloudinary deletion fails
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Deletion failed. File may still exist in Cloudinary.", details = ex.Message });
            }
        }
    }
}

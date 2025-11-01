using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [Route("api/reports")]
    [ApiController]
    // [Authorize] // Uncomment this line when your authentication is set up
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        /// Generates a new report (PDF/Excel), uploads it to Cloudinary, and saves the record
        [HttpPost("generate")]
        [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // FIX: Added [FromQuery] parameters for currentUserId and currentUserRole
        public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequestDto request, [FromQuery] int currentUserId, [FromQuery] UserRole currentUserRole)
        {
            try
            {
                // FIX: Removed hardcoded user/role. We now use the parameters from Swagger.
                _logger.LogInformation("Report generation request received for type {ReportType} by User {UserId}", request.ReportType, currentUserId);

                var reportDto = await _reportService.GenerateAndSaveReport(request, currentUserId, currentUserRole);

                // Return a 201 Created with the location of the new resource
                return CreatedAtAction(nameof(GetReportById), new { id = reportDto.ReportId }, reportDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized report generation attempt by User {UserId}.", currentUserId);
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid report generation request.");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during report generation for User {UserId}.", currentUserId);
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }

      
        /// Gets the report history for the specified user
        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReportHistory([FromQuery] int currentUserId)
        {
            try
            {
                if (currentUserId <= 0)
                    return BadRequest(new { error = "A valid currentUserId must be provided as a query parameter." });

                var reports = await _reportService.GetReportsByUser(currentUserId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report history for User {UserId}.", currentUserId);
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }


        /// Gets a single report's metadata by its ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReportById(int id)
        {
            try
            {
                var report = await _reportService.GetReportById(id);
                if (report == null)
                {
                    return NotFound(new { error = $"Report with ID {id} not found." });
                }

                // Optional: Add authorization check here to ensure the user
                // requesting this ID is allowed to see it (e.g., they generated it or are an Admin).
                // You could pass the [FromQuery] currentUserId to this method as well for that check.

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report by ID {ReportId}.", id);
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }
    }
}


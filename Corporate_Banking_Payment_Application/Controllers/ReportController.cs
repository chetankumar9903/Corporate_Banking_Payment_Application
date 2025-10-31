//using corporate_banking_payment_application.DTOs;
//using Corporate_Banking_Payment_Application.Models;
//using Corporate_Banking_Payment_Application.Services.IService;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace Corporate_Banking_Payment_Application.Controllers
//{
//    [Route("api/reports")]
//    [ApiController]
//    // [Authorize] // Uncomment this line when your authentication is set up
//    public class ReportsController : ControllerBase
//    {
//        private readonly IReportService _reportService;
//        private readonly ILogger<ReportsController> _logger;

//        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
//        {
//            _reportService = reportService;
//            _logger = logger;
//        }

//        /// <summary>
//        /// Generates a new report (PDF/Excel), uploads it to Cloudinary, and saves the record.
//        /// </summary>
//        /// <param name="request">The DTO specifying report parameters.</param>
//        /// <returns>A DTO of the saved report record.</returns>
//        [HttpPost("generate")]
//        [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequestDto request)
//        {
//            try
//            {
//                // --- AUTHENTICATION PLACEHOLDER ---
//                // In a real app, you would get these from the HttpContext.User.Claims
//                // I am hardcoding them here for demonstration.
//                // Replace this with your actual auth logic.
//                var currentUserId = GetUserIdFromClaims();
//                var currentUserRole = GetUserRoleFromClaims();
//                // --- END PLACEHOLDER ---

//                if (currentUserId == -1 || currentUserRole == null)
//                {
//                    return Unauthorized("Could not determine user credentials from token.");
//                }

//                _logger.LogInformation("Report generation request received for type {ReportType} by User {UserId}", request.ReportType, currentUserId);

//                var reportDto = await _reportService.GenerateAndSaveReport(request, currentUserId, currentUserRole.Value);

//                // Return a 201 Created with the location of the new resource
//                return CreatedAtAction(nameof(GetReportById), new { id = reportDto.ReportId }, reportDto);
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                _logger.LogWarning(ex, "Unauthorized report generation attempt by User {UserId}.", GetUserIdFromClaims());
//                return Forbid(ex.Message);
//            }
//            catch (ArgumentException ex)
//            {
//                _logger.LogWarning(ex, "Invalid report generation request.");
//                return BadRequest(new { error = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An unexpected error occurred during report generation for User {UserId}.", GetUserIdFromClaims());
//                return StatusCode(500, new { error = "An internal server error occurred." });
//            }
//        }

//        /// <summary>
//        /// Gets the report history for the currently authenticated user.
//        /// </summary>
//        /// <returns>A list of report metadata objects.</returns>
//        [HttpGet("history")]
//        [ProducesResponseType(typeof(IEnumerable<ReportDto>), StatusCodes.Status200OK)]
//        public async Task<IActionResult> GetReportHistory()
//        {
//            try
//            {
//                var currentUserId = GetUserIdFromClaims();
//                if (currentUserId == -1) return Unauthorized();

//                var reports = await _reportService.GetReportsByUser(currentUserId);
//                return Ok(reports);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving report history for User {UserId}.", GetUserIdFromClaims());
//                return StatusCode(500, new { error = "An internal server error occurred." });
//            }
//        }

//        /// <summary>
//        /// Gets a single report's metadata by its ID.
//        /// </summary>
//        /// <param name="id">The ID of the report to retrieve.</param>
//        /// <returns>The report metadata object.</returns>
//        [HttpGet("{id}")]
//        [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> GetReportById(int id)
//        {
//            try
//            {
//                var report = await _reportService.GetReportById(id);
//                if (report == null)
//                {
//                    return NotFound(new { error = $"Report with ID {id} not found." });
//                }

//                // Optional: Add authorization check here to ensure the user
//                // requesting this ID is allowed to see it (e.g., they generated it or are an Admin).

//                return Ok(report);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving report by ID {ReportId}.", id);
//                return StatusCode(500, new { error = "An internal server error occurred." });
//            }
//        }


//        // --- Authentication Helper Methods (Replace with your real logic) ---

//        private int GetUserIdFromClaims()
//        {
//            // var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
//            // return int.TryParse(userIdClaim, out int userId) ? userId : -1;

//            // Hardcoded placeholder:
//            return 1;
//        }

//        private UserRole? GetUserRoleFromClaims()
//        {
//            // var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
//            // return Enum.TryParse<UserRole>(userRoleClaim, true, out UserRole role) ? role : (UserRole?)null;

//            // Hardcoded placeholder (assuming User 1 is a CLIENTUSER for testing):
//            return UserRole.CLIENTUSER;
//        }
//    }
//}

using corporate_banking_payment_application.DTOs;
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

        /// <summary>
        /// Generates a new report (PDF/Excel), uploads it to Cloudinary, and saves the record.
        /// </summary>
        /// <param name="request">The DTO specifying report parameters.</param>
        /// <param name="currentUserId">The ID of the user generating the report (for testing).</param>
        /// <param name="currentUserRole">The Role of the user generating the report (for testing).</param>
        /// <returns>A DTO of the saved report record.</returns>
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

        /// <summary>
        /// Gets the report history for the specified user.
        /// </summary>
        /// <param name="currentUserId">The ID of the user whose history is being requested (for testing).</param>
        /// <returns>A list of report metadata objects.</returns>
        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<ReportDto>), StatusCodes.Status200OK)]
        // FIX: Added [FromQuery] parameter for currentUserId
        public async Task<IActionResult> GetReportHistory([FromQuery] int currentUserId)
        {
            try
            {
                // FIX: Removed hardcoded user ID.
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

        /// <summary>
        /// Gets a single report's metadata by its ID.
        /// </summary>
        /// <param name="id">The ID of the report to retrieve.</param>
        /// <returns>The report metadata object.</returns>
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


        // FIX: Removed the hardcoded helper methods
        // private int GetUserIdFromClaims() { ... }
        // private UserRole? GetUserRoleFromClaims() { ... }
    }
}


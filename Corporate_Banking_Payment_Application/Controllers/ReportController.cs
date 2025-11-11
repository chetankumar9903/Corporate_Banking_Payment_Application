using CloudinaryDotNet;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Corporate_Banking_Payment_Application.Controllers
{
    [Route("api/reports")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;
        private readonly Cloudinary _cloudinary;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger, IConfiguration config)
        {
            _reportService = reportService;
            _logger = logger;
            var account = new Account(
        config["CloudinarySettings:CloudName"],
        config["CloudinarySettings:ApiKey"],
        config["CloudinarySettings:ApiSecret"]
    );
            _cloudinary = new Cloudinary(account);
        }


        [HttpPost("generate")]
        [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequestDto request, [FromQuery] int currentUserId, [FromQuery] UserRole currentUserRole)
        {
            try
            {

                _logger.LogInformation("Report generation request received for type {ReportType} by User {UserId}", request.ReportType, currentUserId);

                var reportDto = await _reportService.GenerateAndSaveReport(request, currentUserId, currentUserRole);


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




        [HttpGet("history")]
        [ProducesResponseType(typeof(PagedResult<ReportDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReportHistory(
            [FromQuery] int currentUserId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortColumn = null,
            [FromQuery] SortOrder? sortOrder = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (currentUserId <= 0)
                    return BadRequest(new { error = "A valid currentUserId must be provided as a query parameter." });

                var reports = await _reportService.GetReportsByUser(currentUserId, searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report history for User {UserId}.", currentUserId);
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }




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


                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report by ID {ReportId}.", id);
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadReport(int id)
        {
            try
            {

                var report = await _reportService.GetReportById(id);
                if (report == null)
                    return NotFound(new { error = $"Report with ID {id} not found." });


                var uri = new Uri(report.FilePath);
                var fileNameWithExtension = Path.GetFileName(uri.LocalPath);
                var publicId = Path.Combine("corporate_banking_app_reports", Path.GetFileNameWithoutExtension(fileNameWithExtension));


                var downloadUrl = _cloudinary.Api.UrlImgUp
                    .ResourceType("raw")
                    .Type("upload")
                    .BuildUrl(publicId);


                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var signature = _cloudinary.Api.SignParameters(new Dictionary<string, object>
        {
            { "public_id", publicId },
            { "timestamp", timestamp }
        });
                downloadUrl = $"{downloadUrl}?timestamp={timestamp}&signature={signature}&api_key={_cloudinary.Api.Account.ApiKey}";


                using var http = new HttpClient();
                var fileBytes = await http.GetByteArrayAsync(downloadUrl);


                var contentType = report.OutputFormat == ReportOutputFormat.PDF
                    ? "application/pdf"
                    : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


                return File(fileBytes, contentType, fileNameWithExtension);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading report {ReportId}", id);
                return StatusCode(500, new { error = "Failed to download file", details = ex.Message });
            }
        }


    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var stats = await _reportService.GetDashboardStatsAsync(userId);
        return Ok(stats);
    }

    [HttpGet("productivity")]
    public async Task<IActionResult> GetProductivityReport()
    {
        var report = await _reportService.GetProductivityReportAsync();
        return Ok(report);
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int limit = 20)
    {
        var activity = await _reportService.GetRecentActivityAsync(limit);
        return Ok(activity);
    }

    [HttpGet("productivity/export-csv")]
    public async Task<IActionResult> ExportProductivityReportToCsv()
    {
        var csvData = await _reportService.ExportProductivityReportToCsvAsync();
        var fileName = $"ProductivityReport_{DateTime.UtcNow:yyyy-MM-dd_HHmmss}.csv";
        return File(csvData, "text/csv", fileName);
    }

    [HttpGet("dashboard/export-csv")]
    public async Task<IActionResult> ExportDashboardStatsToCsv()
    {
        var csvData = await _reportService.ExportDashboardStatsToCsvAsync();
        var fileName = $"DashboardStats_{DateTime.UtcNow:yyyy-MM-dd_HHmmss}.csv";
        return File(csvData, "text/csv", fileName);
    }

    [HttpGet("activity/export-csv")]
    public async Task<IActionResult> ExportRecentActivityToCsv([FromQuery] int limit = 100)
    {
        var csvData = await _reportService.ExportRecentActivityToCsvAsync(limit);
        var fileName = $"RecentActivity_{DateTime.UtcNow:yyyy-MM-dd_HHmmss}.csv";
        return File(csvData, "text/csv", fileName);
    }
}

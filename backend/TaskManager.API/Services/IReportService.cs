using TaskManager.API.DTOs;

namespace TaskManager.API.Services;

public interface IReportService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync(string? userId = null);
    Task<List<ProductivityReportResponse>> GetProductivityReportAsync();
    Task<List<RecentActivityResponse>> GetRecentActivityAsync(int limit = 20);
    Task<byte[]> ExportProductivityReportToCsvAsync();
    Task<byte[]> ExportDashboardStatsToCsvAsync();
    Task<byte[]> ExportRecentActivityToCsvAsync(int limit = 100);
}

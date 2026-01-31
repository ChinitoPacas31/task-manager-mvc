using TaskManager.API.DTOs;

namespace TaskManager.API.Services;

public interface IReportService
{
    Task<DashboardStatsResponse> GetDashboardStatsAsync(string? userId = null);
    Task<List<ProductivityReportResponse>> GetProductivityReportAsync();
    Task<List<RecentActivityResponse>> GetRecentActivityAsync(int limit = 20);
}

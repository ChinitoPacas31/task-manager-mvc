using MongoDB.Driver;
using TaskManager.API.Data;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace TaskManager.API.Services;

public class ReportService : IReportService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly MongoDbContext _context;

    public ReportService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        MongoDbContext context)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync(string? userId = null)
    {
        var tasks = await _taskRepository.GetAllAsync();
        var projects = await _projectRepository.GetAllAsync();

        var stats = new DashboardStatsResponse
        {
            TotalTasks = tasks.Count,
            CompletedTasks = tasks.Count(t => t.Status == Models.TaskStatus.Completed),
            PendingTasks = tasks.Count(t => t.Status == Models.TaskStatus.Pending),
            InProgressTasks = tasks.Count(t => t.Status == Models.TaskStatus.InProgress),
            HighPriorityTasks = tasks.Count(t => t.Priority == TaskPriority.High || t.Priority == TaskPriority.Critical),
            OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != Models.TaskStatus.Completed && t.Status != Models.TaskStatus.Cancelled),
            TotalProjects = projects.Count,
            ActiveProjects = projects.Count(p => p.Status == ProjectStatus.Active)
        };

        // Tasks by status
        var statusCounts = await _taskRepository.GetTaskCountsByStatusAsync();
        stats.TasksByStatus = statusCounts.Select(kv => new TasksByStatusResponse
        {
            Status = kv.Key,
            Count = kv.Value
        }).ToList();

        // Tasks by priority
        var priorityCounts = await _taskRepository.GetTaskCountsByPriorityAsync();
        stats.TasksByPriority = priorityCounts.Select(kv => new TasksByPriorityResponse
        {
            Priority = kv.Key,
            Count = kv.Value
        }).ToList();

        // Tasks by project
        stats.TasksByProject = new List<TasksByProjectResponse>();
        foreach (var project in projects.Take(10))
        {
            var projectTasks = tasks.Where(t => t.ProjectId == project.Id).ToList();
            stats.TasksByProject.Add(new TasksByProjectResponse
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                TotalTasks = projectTasks.Count,
                CompletedTasks = projectTasks.Count(t => t.Status == Models.TaskStatus.Completed)
            });
        }

        // Recent activity
        stats.RecentActivity = await GetRecentActivityAsync(10);

        return stats;
    }

    public async Task<List<ProductivityReportResponse>> GetProductivityReportAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var tasks = await _taskRepository.GetAllAsync();
        var reports = new List<ProductivityReportResponse>();

        foreach (var user in users)
        {
            var userTasks = tasks.Where(t => t.AssignedToId == user.Id).ToList();
            var completedTasks = userTasks.Where(t => t.Status == Models.TaskStatus.Completed).ToList();

            decimal avgCompletionTime = 0;
            if (completedTasks.Any())
            {
                var completionTimes = completedTasks
                    .Where(t => t.CompletedAt.HasValue)
                    .Select(t => (t.CompletedAt!.Value - t.CreatedAt).TotalHours);
                
                if (completionTimes.Any())
                {
                    avgCompletionTime = (decimal)completionTimes.Average();
                }
            }

            reports.Add(new ProductivityReportResponse
            {
                UserId = user.Id,
                UserName = user.FullName,
                TasksAssigned = userTasks.Count,
                TasksCompleted = completedTasks.Count,
                AverageCompletionTime = Math.Round(avgCompletionTime, 2),
                TotalHoursWorked = completedTasks.Sum(t => t.ActualHours ?? 0)
            });
        }

        return reports.OrderByDescending(r => r.TasksCompleted).ToList();
    }

    public async Task<List<RecentActivityResponse>> GetRecentActivityAsync(int limit = 20)
    {
        var histories = await _context.TaskHistories
            .Find(_ => true)
            .SortByDescending(h => h.CreatedAt)
            .Limit(limit)
            .ToListAsync();

        var responses = new List<RecentActivityResponse>();
        foreach (var history in histories)
        {
            var user = await _userRepository.GetByIdAsync(history.UserId);
            var task = await _taskRepository.GetByIdAsync(history.TaskId);

            responses.Add(new RecentActivityResponse
            {
                Id = history.Id,
                TaskId = history.TaskId,
                TaskTitle = task?.Title ?? "Unknown",
                Action = history.Action.ToString(),
                Description = history.Description,
                User = new UserBasicResponse
                {
                    Id = user?.Id ?? "",
                    Username = user?.Username ?? "Unknown",
                    FullName = user?.FullName ?? "Unknown"
                },
                CreatedAt = history.CreatedAt
            });
        }

        return responses;
    }

    public async Task<byte[]> ExportProductivityReportToCsvAsync()
    {
        var report = await GetProductivityReportAsync();
        
        using (var memoryStream = new MemoryStream())
        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(report.ConvertAll(r => new
            {
                r.UserId,
                r.UserName,
                r.TasksAssigned,
                r.TasksCompleted,
                AverageCompletionTimeHours = r.AverageCompletionTime,
                r.TotalHoursWorked
            }));
            
            writer.Flush();
            return memoryStream.ToArray();
        }
    }

    public async Task<byte[]> ExportDashboardStatsToCsvAsync()
    {
        var stats = await GetDashboardStatsAsync();
        
        using (var memoryStream = new MemoryStream())
        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            // Write main stats
            var mainStats = new[]
            {
                new { Metric = "Total Tasks", Value = stats.TotalTasks.ToString() },
                new { Metric = "Completed Tasks", Value = stats.CompletedTasks.ToString() },
                new { Metric = "Pending Tasks", Value = stats.PendingTasks.ToString() },
                new { Metric = "In Progress Tasks", Value = stats.InProgressTasks.ToString() },
                new { Metric = "High Priority Tasks", Value = stats.HighPriorityTasks.ToString() },
                new { Metric = "Overdue Tasks", Value = stats.OverdueTasks.ToString() },
                new { Metric = "Total Projects", Value = stats.TotalProjects.ToString() },
                new { Metric = "Active Projects", Value = stats.ActiveProjects.ToString() }
            };
            
            csvWriter.WriteRecords(mainStats);
            writer.WriteLine();
            
            // Write tasks by status
            writer.WriteLine("Tasks by Status");
            csvWriter.WriteRecords(stats.TasksByStatus.ConvertAll(ts => new
            {
                ts.Status,
                ts.Count
            }));
            writer.WriteLine();
            
            // Write tasks by priority
            writer.WriteLine("Tasks by Priority");
            csvWriter.WriteRecords(stats.TasksByPriority.ConvertAll(tp => new
            {
                tp.Priority,
                tp.Count
            }));
            writer.WriteLine();
            
            // Write tasks by project
            writer.WriteLine("Tasks by Project");
            csvWriter.WriteRecords(stats.TasksByProject.ConvertAll(tp => new
            {
                tp.ProjectName,
                tp.TotalTasks,
                tp.CompletedTasks
            }));
            
            writer.Flush();
            return memoryStream.ToArray();
        }
    }

    public async Task<byte[]> ExportRecentActivityToCsvAsync(int limit = 100)
    {
        var activity = await GetRecentActivityAsync(limit);
        
        using (var memoryStream = new MemoryStream())
        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(activity.ConvertAll(a => new
            {
                a.TaskTitle,
                a.Action,
                a.Description,
                UserName = a.User?.FullName ?? "Unknown",
                a.CreatedAt
            }));
            
            writer.Flush();
            return memoryStream.ToArray();
        }
    }
}

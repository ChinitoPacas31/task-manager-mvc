namespace TaskManager.API.DTOs;

public class DashboardStatsResponse
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int HighPriorityTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public List<TasksByStatusResponse> TasksByStatus { get; set; } = new();
    public List<TasksByPriorityResponse> TasksByPriority { get; set; } = new();
    public List<TasksByProjectResponse> TasksByProject { get; set; } = new();
    public List<RecentActivityResponse> RecentActivity { get; set; } = new();
}

public class TasksByStatusResponse
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TasksByPriorityResponse
{
    public string Priority { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TasksByProjectResponse
{
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
}

public class RecentActivityResponse
{
    public string Id { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public string TaskTitle { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UserBasicResponse User { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class ProductivityReportResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int TasksCompleted { get; set; }
    public int TasksAssigned { get; set; }
    public decimal AverageCompletionTime { get; set; }
    public decimal TotalHoursWorked { get; set; }
}

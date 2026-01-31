using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;
using TaskManager.API.Data;

namespace TaskManager.API.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly INotificationService _notificationService;
    private readonly MongoDbContext _context;

    public TaskService(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        ICommentRepository commentRepository,
        INotificationService notificationService,
        MongoDbContext context)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _commentRepository = commentRepository;
        _notificationService = notificationService;
        _context = context;
    }

    public async Task<TaskResponse?> GetByIdAsync(string id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null) return null;

        return await MapToResponseAsync(task);
    }

    public async Task<TaskListResponse> GetFilteredAsync(TaskFilterRequest filter)
    {
        var (tasks, totalCount) = await _taskRepository.GetFilteredAsync(filter);
        var responses = new List<TaskResponse>();

        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }

        return new TaskListResponse
        {
            Tasks = responses,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
        };
    }

    public async Task<List<TaskResponse>> GetByProjectIdAsync(string projectId)
    {
        var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
        var responses = new List<TaskResponse>();

        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }

        return responses;
    }

    public async Task<List<TaskResponse>> GetByAssignedUserIdAsync(string userId)
    {
        var tasks = await _taskRepository.GetByAssignedUserIdAsync(userId);
        var responses = new List<TaskResponse>();

        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }

        return responses;
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, string createdById)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = Enum.TryParse<Models.TaskStatus>(request.Status, out var status) ? status : Models.TaskStatus.Pending,
            Priority = Enum.TryParse<TaskPriority>(request.Priority, out var priority) ? priority : TaskPriority.Medium,
            ProjectId = request.ProjectId,
            AssignedToId = request.AssignedToId,
            CreatedById = createdById,
            DueDate = request.DueDate,
            EstimatedHours = request.EstimatedHours,
            Tags = request.Tags ?? new List<string>(),
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.CreateAsync(task);

        // Create history entry
        await CreateHistoryEntry(task.Id, createdById, HistoryAction.Created, null, null, null, $"Tarea '{task.Title}' creada");

        // Send notification if assigned to someone
        if (!string.IsNullOrEmpty(request.AssignedToId))
        {
            await _notificationService.CreateAsync(
                request.AssignedToId,
                "Nueva tarea asignada",
                $"Se te ha asignado la tarea '{task.Title}'",
                NotificationType.TaskAssigned,
                task.Id,
                task.ProjectId);
        }

        return await MapToResponseAsync(task);
    }

    public async Task<TaskResponse?> UpdateAsync(string id, UpdateTaskRequest request, string userId)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null) return null;

        var oldStatus = task.Status;
        var oldAssignedTo = task.AssignedToId;

        if (request.Title != null) task.Title = request.Title;
        if (request.Description != null) task.Description = request.Description;
        if (request.Status != null && Enum.TryParse<Models.TaskStatus>(request.Status, out var status))
        {
            task.Status = status;
            if (status == Models.TaskStatus.Completed)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
        }
        if (request.Priority != null && Enum.TryParse<TaskPriority>(request.Priority, out var priority))
        {
            task.Priority = priority;
        }
        if (request.ProjectId != null) task.ProjectId = request.ProjectId;
        if (request.AssignedToId != null) task.AssignedToId = request.AssignedToId;
        if (request.DueDate.HasValue) task.DueDate = request.DueDate;
        if (request.EstimatedHours.HasValue) task.EstimatedHours = request.EstimatedHours;
        if (request.ActualHours.HasValue) task.ActualHours = request.ActualHours;
        if (request.Tags != null) task.Tags = request.Tags;

        await _taskRepository.UpdateAsync(task);

        // Create history entries for changes
        if (oldStatus != task.Status)
        {
            await CreateHistoryEntry(task.Id, userId, HistoryAction.StatusChanged, "Status", 
                oldStatus.ToString(), task.Status.ToString(), 
                $"Estado cambiado de '{oldStatus}' a '{task.Status}'");
        }

        if (oldAssignedTo != task.AssignedToId && !string.IsNullOrEmpty(task.AssignedToId))
        {
            await CreateHistoryEntry(task.Id, userId, HistoryAction.Assigned, "AssignedTo", 
                oldAssignedTo, task.AssignedToId, "Tarea reasignada");

            await _notificationService.CreateAsync(
                task.AssignedToId,
                "Tarea asignada",
                $"Se te ha asignado la tarea '{task.Title}'",
                NotificationType.TaskAssigned,
                task.Id,
                task.ProjectId);
        }

        return await MapToResponseAsync(task);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _taskRepository.DeleteAsync(id);
    }

    public async Task<List<TaskResponse>> SearchAsync(string searchTerm)
    {
        var filter = new TaskFilterRequest { SearchTerm = searchTerm, PageSize = 100 };
        var (tasks, _) = await _taskRepository.GetFilteredAsync(filter);
        var responses = new List<TaskResponse>();

        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }

        return responses;
    }

    private async Task<TaskResponse> MapToResponseAsync(TaskItem task)
    {
        var createdBy = await _userRepository.GetByIdAsync(task.CreatedById);
        User? assignedTo = null;
        Project? project = null;

        if (!string.IsNullOrEmpty(task.AssignedToId))
        {
            assignedTo = await _userRepository.GetByIdAsync(task.AssignedToId);
        }

        if (!string.IsNullOrEmpty(task.ProjectId))
        {
            project = await _projectRepository.GetByIdAsync(task.ProjectId);
        }

        var commentCount = await _commentRepository.CountByTaskIdAsync(task.Id);

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            Project = project != null ? new ProjectBasicResponse
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color
            } : null,
            AssignedTo = assignedTo != null ? new UserBasicResponse
            {
                Id = assignedTo.Id,
                Username = assignedTo.Username,
                FullName = assignedTo.FullName
            } : null,
            CreatedBy = new UserBasicResponse
            {
                Id = createdBy?.Id ?? "",
                Username = createdBy?.Username ?? "Unknown",
                FullName = createdBy?.FullName ?? "Unknown"
            },
            DueDate = task.DueDate,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            Tags = task.Tags,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            CompletedAt = task.CompletedAt,
            CommentCount = commentCount
        };
    }

    private async Task CreateHistoryEntry(string taskId, string userId, HistoryAction action, 
        string? field, string? oldValue, string? newValue, string description)
    {
        var history = new TaskHistory
        {
            TaskId = taskId,
            UserId = userId,
            Action = action,
            Field = field,
            OldValue = oldValue,
            NewValue = newValue,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TaskHistories.InsertOneAsync(history);
    }
}

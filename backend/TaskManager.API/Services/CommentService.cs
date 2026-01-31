using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;

namespace TaskManager.API.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly INotificationService _notificationService;

    public CommentService(
        ICommentRepository commentRepository,
        IUserRepository userRepository,
        ITaskRepository taskRepository,
        INotificationService notificationService)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _taskRepository = taskRepository;
        _notificationService = notificationService;
    }

    public async Task<CommentResponse?> GetByIdAsync(string id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null) return null;

        return await MapToResponseAsync(comment);
    }

    public async Task<List<CommentResponse>> GetByTaskIdAsync(string taskId)
    {
        var comments = await _commentRepository.GetByTaskIdAsync(taskId);
        var responses = new List<CommentResponse>();

        foreach (var comment in comments)
        {
            responses.Add(await MapToResponseAsync(comment));
        }

        return responses;
    }

    public async Task<CommentResponse> CreateAsync(CreateCommentRequest request, string userId)
    {
        var comment = new Comment
        {
            TaskId = request.TaskId,
            UserId = userId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.CreateAsync(comment);

        // Notify task assignee and creator
        var task = await _taskRepository.GetByIdAsync(request.TaskId);
        if (task != null)
        {
            var notifyUsers = new HashSet<string>();
            
            if (!string.IsNullOrEmpty(task.AssignedToId) && task.AssignedToId != userId)
            {
                notifyUsers.Add(task.AssignedToId);
            }
            
            if (task.CreatedById != userId)
            {
                notifyUsers.Add(task.CreatedById);
            }

            var commenter = await _userRepository.GetByIdAsync(userId);
            foreach (var notifyUserId in notifyUsers)
            {
                await _notificationService.CreateAsync(
                    notifyUserId,
                    "Nuevo comentario",
                    $"{commenter?.FullName ?? "Alguien"} comentó en la tarea '{task.Title}'",
                    NotificationType.CommentAdded,
                    task.Id,
                    task.ProjectId);
            }
        }

        return await MapToResponseAsync(comment);
    }

    public async Task<CommentResponse?> UpdateAsync(string id, UpdateCommentRequest request, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null || comment.UserId != userId) return null;

        comment.Content = request.Content;
        await _commentRepository.UpdateAsync(comment);

        return await MapToResponseAsync(comment);
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null || comment.UserId != userId) return false;

        return await _commentRepository.DeleteAsync(id);
    }

    private async Task<CommentResponse> MapToResponseAsync(Comment comment)
    {
        var user = await _userRepository.GetByIdAsync(comment.UserId);

        return new CommentResponse
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            User = new UserBasicResponse
            {
                Id = user?.Id ?? "",
                Username = user?.Username ?? "Unknown",
                FullName = user?.FullName ?? "Unknown"
            },
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            IsEdited = comment.IsEdited
        };
    }
}

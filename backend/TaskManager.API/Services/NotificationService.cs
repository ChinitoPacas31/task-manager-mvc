using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;

namespace TaskManager.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationResponse>> GetByUserIdAsync(string userId)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        return notifications.Select(MapToResponse).ToList();
    }

    public async Task<NotificationCountResponse> GetCountAsync(string userId)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        var unreadCount = await _notificationRepository.CountUnreadByUserIdAsync(userId);

        return new NotificationCountResponse
        {
            Total = notifications.Count,
            Unread = unreadCount
        };
    }

    public async Task<NotificationResponse> CreateAsync(string userId, string title, string message,
        NotificationType type, string? relatedTaskId = null, string? relatedProjectId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedTaskId = relatedTaskId,
            RelatedProjectId = relatedProjectId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.CreateAsync(notification);
        return MapToResponse(notification);
    }

    public async Task<bool> MarkAsReadAsync(string id)
    {
        return await _notificationRepository.MarkAsReadAsync(id);
    }

    public async Task<bool> MarkAllAsReadAsync(string userId)
    {
        return await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _notificationRepository.DeleteAsync(id);
    }

    private NotificationResponse MapToResponse(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            RelatedTaskId = notification.RelatedTaskId,
            RelatedProjectId = notification.RelatedProjectId,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}

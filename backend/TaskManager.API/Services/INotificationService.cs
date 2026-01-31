using TaskManager.API.DTOs;
using TaskManager.API.Models;

namespace TaskManager.API.Services;

public interface INotificationService
{
    Task<List<NotificationResponse>> GetByUserIdAsync(string userId);
    Task<NotificationCountResponse> GetCountAsync(string userId);
    Task<NotificationResponse> CreateAsync(string userId, string title, string message, 
        NotificationType type, string? relatedTaskId = null, string? relatedProjectId = null);
    Task<bool> MarkAsReadAsync(string id);
    Task<bool> MarkAllAsReadAsync(string userId);
    Task<bool> DeleteAsync(string id);
}

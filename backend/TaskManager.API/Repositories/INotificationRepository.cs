using TaskManager.API.Models;

namespace TaskManager.API.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(string id);
    Task<List<Notification>> GetByUserIdAsync(string userId, int limit = 50);
    Task<List<Notification>> GetUnreadByUserIdAsync(string userId);
    Task<Notification> CreateAsync(Notification notification);
    Task<bool> MarkAsReadAsync(string id);
    Task<bool> MarkAllAsReadAsync(string userId);
    Task<bool> DeleteAsync(string id);
    Task<int> CountUnreadByUserIdAsync(string userId);
}

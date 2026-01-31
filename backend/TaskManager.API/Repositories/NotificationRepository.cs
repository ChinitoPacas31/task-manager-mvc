using MongoDB.Driver;
using TaskManager.API.Data;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly MongoDbContext _context;

    public NotificationRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(string id)
    {
        return await _context.Notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Notification>> GetByUserIdAsync(string userId, int limit = 50)
    {
        return await _context.Notifications
            .Find(n => n.UserId == userId)
            .SortByDescending(n => n.CreatedAt)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetUnreadByUserIdAsync(string userId)
    {
        return await _context.Notifications
            .Find(n => n.UserId == userId && !n.IsRead)
            .SortByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        await _context.Notifications.InsertOneAsync(notification);
        return notification;
    }

    public async Task<bool> MarkAsReadAsync(string id)
    {
        var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
        var result = await _context.Notifications.UpdateOneAsync(n => n.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> MarkAllAsReadAsync(string userId)
    {
        var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
        var result = await _context.Notifications.UpdateManyAsync(n => n.UserId == userId && !n.IsRead, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _context.Notifications.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<int> CountUnreadByUserIdAsync(string userId)
    {
        return (int)await _context.Notifications.CountDocumentsAsync(n => n.UserId == userId && !n.IsRead);
    }
}

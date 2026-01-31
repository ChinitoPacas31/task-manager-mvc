using MongoDB.Driver;
using TaskManager.API.Data;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly MongoDbContext _context;

    public CommentRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Comment?> GetByIdAsync(string id)
    {
        return await _context.Comments.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Comment>> GetByTaskIdAsync(string taskId)
    {
        return await _context.Comments
            .Find(c => c.TaskId == taskId)
            .SortByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        await _context.Comments.InsertOneAsync(comment);
        return comment;
    }

    public async Task<bool> UpdateAsync(Comment comment)
    {
        comment.UpdatedAt = DateTime.UtcNow;
        comment.IsEdited = true;
        var result = await _context.Comments.ReplaceOneAsync(c => c.Id == comment.Id, comment);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _context.Comments.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<int> CountByTaskIdAsync(string taskId)
    {
        return (int)await _context.Comments.CountDocumentsAsync(c => c.TaskId == taskId);
    }
}

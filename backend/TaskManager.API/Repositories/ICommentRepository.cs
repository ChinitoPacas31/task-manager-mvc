using TaskManager.API.Models;

namespace TaskManager.API.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(string id);
    Task<List<Comment>> GetByTaskIdAsync(string taskId);
    Task<Comment> CreateAsync(Comment comment);
    Task<bool> UpdateAsync(Comment comment);
    Task<bool> DeleteAsync(string id);
    Task<int> CountByTaskIdAsync(string taskId);
}

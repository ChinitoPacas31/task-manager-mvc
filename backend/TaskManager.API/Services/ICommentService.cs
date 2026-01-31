using TaskManager.API.DTOs;

namespace TaskManager.API.Services;

public interface ICommentService
{
    Task<CommentResponse?> GetByIdAsync(string id);
    Task<List<CommentResponse>> GetByTaskIdAsync(string taskId);
    Task<CommentResponse> CreateAsync(CreateCommentRequest request, string userId);
    Task<CommentResponse?> UpdateAsync(string id, UpdateCommentRequest request, string userId);
    Task<bool> DeleteAsync(string id, string userId);
}

using TaskManager.API.DTOs;

namespace TaskManager.API.Services;

public interface ITaskService
{
    Task<TaskResponse?> GetByIdAsync(string id);
    Task<TaskListResponse> GetFilteredAsync(TaskFilterRequest filter);
    Task<List<TaskResponse>> GetByProjectIdAsync(string projectId);
    Task<List<TaskResponse>> GetByAssignedUserIdAsync(string userId);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, string createdById);
    Task<TaskResponse?> UpdateAsync(string id, UpdateTaskRequest request, string userId);
    Task<bool> DeleteAsync(string id);
    Task<List<TaskResponse>> SearchAsync(string searchTerm);
}

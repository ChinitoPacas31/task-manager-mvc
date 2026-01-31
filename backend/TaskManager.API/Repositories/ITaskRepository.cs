using TaskManager.API.Models;
using TaskManager.API.DTOs;

namespace TaskManager.API.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(string id);
    Task<List<TaskItem>> GetAllAsync();
    Task<(List<TaskItem> Tasks, int TotalCount)> GetFilteredAsync(TaskFilterRequest filter);
    Task<List<TaskItem>> GetByProjectIdAsync(string projectId);
    Task<List<TaskItem>> GetByAssignedUserIdAsync(string userId);
    Task<List<TaskItem>> GetOverdueTasksAsync();
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<bool> UpdateAsync(TaskItem task);
    Task<bool> DeleteAsync(string id);
    Task<int> CountByStatusAsync(Models.TaskStatus status);
    Task<int> CountByPriorityAsync(TaskPriority priority);
    Task<int> CountByProjectAsync(string projectId);
    Task<Dictionary<string, int>> GetTaskCountsByStatusAsync();
    Task<Dictionary<string, int>> GetTaskCountsByPriorityAsync();
}

using TaskManager.API.Models;

namespace TaskManager.API.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(string id);
    Task<List<Project>> GetAllAsync();
    Task<List<Project>> GetByOwnerIdAsync(string ownerId);
    Task<List<Project>> GetByMemberIdAsync(string memberId);
    Task<Project> CreateAsync(Project project);
    Task<bool> UpdateAsync(Project project);
    Task<bool> DeleteAsync(string id);
    Task<int> CountAsync();
    Task<int> CountByStatusAsync(ProjectStatus status);
}

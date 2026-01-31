using TaskManager.API.DTOs;

namespace TaskManager.API.Services;

public interface IProjectService
{
    Task<ProjectResponse?> GetByIdAsync(string id);
    Task<List<ProjectResponse>> GetAllAsync();
    Task<List<ProjectResponse>> GetByUserIdAsync(string userId);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request, string ownerId);
    Task<ProjectResponse?> UpdateAsync(string id, UpdateProjectRequest request);
    Task<bool> DeleteAsync(string id);
}

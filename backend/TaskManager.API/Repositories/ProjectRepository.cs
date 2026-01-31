using MongoDB.Driver;
using TaskManager.API.Data;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly MongoDbContext _context;

    public ProjectRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(string id)
    {
        return await _context.Projects.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects.Find(_ => true).ToListAsync();
    }

    public async Task<List<Project>> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Projects.Find(p => p.OwnerId == ownerId).ToListAsync();
    }

    public async Task<List<Project>> GetByMemberIdAsync(string memberId)
    {
        return await _context.Projects
            .Find(p => p.MemberIds.Contains(memberId) || p.OwnerId == memberId)
            .ToListAsync();
    }

    public async Task<Project> CreateAsync(Project project)
    {
        await _context.Projects.InsertOneAsync(project);
        return project;
    }

    public async Task<bool> UpdateAsync(Project project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        var result = await _context.Projects.ReplaceOneAsync(p => p.Id == project.Id, project);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _context.Projects.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<int> CountAsync()
    {
        return (int)await _context.Projects.CountDocumentsAsync(_ => true);
    }

    public async Task<int> CountByStatusAsync(ProjectStatus status)
    {
        return (int)await _context.Projects.CountDocumentsAsync(p => p.Status == status);
    }
}

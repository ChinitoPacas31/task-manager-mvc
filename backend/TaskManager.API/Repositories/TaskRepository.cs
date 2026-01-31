using MongoDB.Driver;
using TaskManager.API.Data;
using TaskManager.API.Models;
using TaskManager.API.DTOs;

namespace TaskManager.API.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly MongoDbContext _context;

    public TaskRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(string id)
    {
        return await _context.Tasks.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks.Find(_ => true).ToListAsync();
    }

    public async Task<(List<TaskItem> Tasks, int TotalCount)> GetFilteredAsync(TaskFilterRequest filter)
    {
        var filterBuilder = Builders<TaskItem>.Filter;
        var filters = new List<FilterDefinition<TaskItem>>();

        if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<Models.TaskStatus>(filter.Status, out var status))
        {
            filters.Add(filterBuilder.Eq(t => t.Status, status));
        }

        if (!string.IsNullOrEmpty(filter.Priority) && Enum.TryParse<TaskPriority>(filter.Priority, out var priority))
        {
            filters.Add(filterBuilder.Eq(t => t.Priority, priority));
        }

        if (!string.IsNullOrEmpty(filter.ProjectId))
        {
            filters.Add(filterBuilder.Eq(t => t.ProjectId, filter.ProjectId));
        }

        if (!string.IsNullOrEmpty(filter.AssignedToId))
        {
            filters.Add(filterBuilder.Eq(t => t.AssignedToId, filter.AssignedToId));
        }

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var searchFilter = filterBuilder.Or(
                filterBuilder.Regex(t => t.Title, new MongoDB.Bson.BsonRegularExpression(filter.SearchTerm, "i")),
                filterBuilder.Regex(t => t.Description, new MongoDB.Bson.BsonRegularExpression(filter.SearchTerm, "i"))
            );
            filters.Add(searchFilter);
        }

        if (filter.DueDateFrom.HasValue)
        {
            filters.Add(filterBuilder.Gte(t => t.DueDate, filter.DueDateFrom.Value));
        }

        if (filter.DueDateTo.HasValue)
        {
            filters.Add(filterBuilder.Lte(t => t.DueDate, filter.DueDateTo.Value));
        }

        var combinedFilter = filters.Count > 0 
            ? filterBuilder.And(filters) 
            : filterBuilder.Empty;

        var totalCount = await _context.Tasks.CountDocumentsAsync(combinedFilter);

        var sortDefinition = filter.SortDescending
            ? Builders<TaskItem>.Sort.Descending(filter.SortBy)
            : Builders<TaskItem>.Sort.Ascending(filter.SortBy);

        var tasks = await _context.Tasks
            .Find(combinedFilter)
            .Sort(sortDefinition)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Limit(filter.PageSize)
            .ToListAsync();

        return (tasks, (int)totalCount);
    }

    public async Task<List<TaskItem>> GetByProjectIdAsync(string projectId)
    {
        return await _context.Tasks.Find(t => t.ProjectId == projectId).ToListAsync();
    }

    public async Task<List<TaskItem>> GetByAssignedUserIdAsync(string userId)
    {
        return await _context.Tasks.Find(t => t.AssignedToId == userId).ToListAsync();
    }

    public async Task<List<TaskItem>> GetOverdueTasksAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Tasks
            .Find(t => t.DueDate < now && t.Status != Models.TaskStatus.Completed && t.Status != Models.TaskStatus.Cancelled)
            .ToListAsync();
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        await _context.Tasks.InsertOneAsync(task);
        return task;
    }

    public async Task<bool> UpdateAsync(TaskItem task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        var result = await _context.Tasks.ReplaceOneAsync(t => t.Id == task.Id, task);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _context.Tasks.DeleteOneAsync(t => t.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<int> CountByStatusAsync(Models.TaskStatus status)
    {
        return (int)await _context.Tasks.CountDocumentsAsync(t => t.Status == status);
    }

    public async Task<int> CountByPriorityAsync(TaskPriority priority)
    {
        return (int)await _context.Tasks.CountDocumentsAsync(t => t.Priority == priority);
    }

    public async Task<int> CountByProjectAsync(string projectId)
    {
        return (int)await _context.Tasks.CountDocumentsAsync(t => t.ProjectId == projectId);
    }

    public async Task<Dictionary<string, int>> GetTaskCountsByStatusAsync()
    {
        var result = new Dictionary<string, int>();
        foreach (Models.TaskStatus status in Enum.GetValues(typeof(Models.TaskStatus)))
        {
            var count = await CountByStatusAsync(status);
            result[status.ToString()] = count;
        }
        return result;
    }

    public async Task<Dictionary<string, int>> GetTaskCountsByPriorityAsync()
    {
        var result = new Dictionary<string, int>();
        foreach (TaskPriority priority in Enum.GetValues(typeof(TaskPriority)))
        {
            var count = await CountByPriorityAsync(priority);
            result[priority.ToString()] = count;
        }
        return result;
    }
}

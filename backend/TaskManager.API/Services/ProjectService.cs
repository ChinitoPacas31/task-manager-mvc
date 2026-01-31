using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;

namespace TaskManager.API.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ITaskRepository taskRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _taskRepository = taskRepository;
    }

    public async Task<ProjectResponse?> GetByIdAsync(string id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null) return null;

        return await MapToResponseAsync(project);
    }

    public async Task<List<ProjectResponse>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        var responses = new List<ProjectResponse>();

        foreach (var project in projects)
        {
            responses.Add(await MapToResponseAsync(project));
        }

        return responses;
    }

    public async Task<List<ProjectResponse>> GetByUserIdAsync(string userId)
    {
        var projects = await _projectRepository.GetByMemberIdAsync(userId);
        var responses = new List<ProjectResponse>();

        foreach (var project in projects)
        {
            responses.Add(await MapToResponseAsync(project));
        }

        return responses;
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request, string ownerId)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Status = ProjectStatus.Active,
            OwnerId = ownerId,
            MemberIds = request.MemberIds ?? new List<string>(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Color = request.Color,
            CreatedAt = DateTime.UtcNow
        };

        await _projectRepository.CreateAsync(project);
        return await MapToResponseAsync(project);
    }

    public async Task<ProjectResponse?> UpdateAsync(string id, UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null) return null;

        if (request.Name != null) project.Name = request.Name;
        if (request.Description != null) project.Description = request.Description;
        if (request.Status != null && Enum.TryParse<ProjectStatus>(request.Status, out var status))
        {
            project.Status = status;
        }
        if (request.StartDate.HasValue) project.StartDate = request.StartDate;
        if (request.EndDate.HasValue) project.EndDate = request.EndDate;
        if (request.Color != null) project.Color = request.Color;
        if (request.MemberIds != null) project.MemberIds = request.MemberIds;

        await _projectRepository.UpdateAsync(project);
        return await MapToResponseAsync(project);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _projectRepository.DeleteAsync(id);
    }

    private async Task<ProjectResponse> MapToResponseAsync(Project project)
    {
        var owner = await _userRepository.GetByIdAsync(project.OwnerId);
        var members = new List<UserBasicResponse>();

        foreach (var memberId in project.MemberIds)
        {
            var member = await _userRepository.GetByIdAsync(memberId);
            if (member != null)
            {
                members.Add(new UserBasicResponse
                {
                    Id = member.Id,
                    Username = member.Username,
                    FullName = member.FullName
                });
            }
        }

        var projectTasks = await _taskRepository.GetByProjectIdAsync(project.Id);
        var stats = new ProjectStats
        {
            TotalTasks = projectTasks.Count,
            CompletedTasks = projectTasks.Count(t => t.Status == Models.TaskStatus.Completed),
            PendingTasks = projectTasks.Count(t => t.Status == Models.TaskStatus.Pending),
            InProgressTasks = projectTasks.Count(t => t.Status == Models.TaskStatus.InProgress),
            OverdueTasks = projectTasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != Models.TaskStatus.Completed)
        };
        stats.CompletionPercentage = stats.TotalTasks > 0 
            ? Math.Round((decimal)stats.CompletedTasks / stats.TotalTasks * 100, 2) 
            : 0;

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status.ToString(),
            Owner = new UserBasicResponse
            {
                Id = owner?.Id ?? "",
                Username = owner?.Username ?? "Unknown",
                FullName = owner?.FullName ?? "Unknown"
            },
            Members = members,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Color = project.Color,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Stats = stats
        };
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var project = await _projectService.GetByIdAsync(id);
        
        if (project == null)
        {
            return NotFound(new { message = "Proyecto no encontrado" });
        }

        return Ok(project);
    }

    [HttpGet("my-projects")]
    public async Task<IActionResult> GetMyProjects()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var projects = await _projectService.GetByUserIdAsync(userId);
        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var project = await _projectService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateProjectRequest request)
    {
        var project = await _projectService.UpdateAsync(id, request);
        
        if (project == null)
        {
            return NotFound(new { message = "Proyecto no encontrado" });
        }

        return Ok(project);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _projectService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = "Proyecto no encontrado" });
        }

        return NoContent();
    }
}

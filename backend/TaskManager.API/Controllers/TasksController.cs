using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskFilterRequest filter)
    {
        var result = await _taskService.GetFilteredAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var task = await _taskService.GetByIdAsync(id);
        
        if (task == null)
        {
            return NotFound(new { message = "Tarea no encontrada" });
        }

        return Ok(task);
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(string projectId)
    {
        var tasks = await _taskService.GetByProjectIdAsync(projectId);
        return Ok(tasks);
    }

    [HttpGet("assigned/{userId}")]
    public async Task<IActionResult> GetByAssignedUser(string userId)
    {
        var tasks = await _taskService.GetByAssignedUserIdAsync(userId);
        return Ok(tasks);
    }

    [HttpGet("my-tasks")]
    public async Task<IActionResult> GetMyTasks()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var tasks = await _taskService.GetByAssignedUserIdAsync(userId);
        return Ok(tasks);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Término de búsqueda requerido" });
        }

        var tasks = await _taskService.SearchAsync(q);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var task = await _taskService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTaskRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var task = await _taskService.UpdateAsync(id, request, userId);
        
        if (task == null)
        {
            return NotFound(new { message = "Tarea no encontrada" });
        }

        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _taskService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = "Tarea no encontrada" });
        }

        return NoContent();
    }
}

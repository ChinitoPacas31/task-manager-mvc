using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetByTask(string taskId)
    {
        var comments = await _commentService.GetByTaskIdAsync(taskId);
        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var comment = await _commentService.GetByIdAsync(id);
        
        if (comment == null)
        {
            return NotFound(new { message = "Comentario no encontrado" });
        }

        return Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var comment = await _commentService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCommentRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var comment = await _commentService.UpdateAsync(id, request, userId);
        
        if (comment == null)
        {
            return NotFound(new { message = "Comentario no encontrado o no tienes permisos" });
        }

        return Ok(comment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _commentService.DeleteAsync(id, userId);
        
        if (!result)
        {
            return NotFound(new { message = "Comentario no encontrado o no tienes permisos" });
        }

        return NoContent();
    }
}

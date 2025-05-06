using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Models;

namespace TaskManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly TaskManagementDbContext _context;

    public TasksController(TaskManagementDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var task = new task()
        {
            userid = userId,
            title = dto.Title,
            description = dto.Description,
            status = dto.Status,
            duedate = dto.DueDate,
            createdat = DateTime.Now,
            updatedat = DateTime.Now
        };
        _context.tasks.Add(task);
        await _context.SaveChangesAsync();
        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(string? status, string? sortBy = "DueDate", string? sortOrder = "asc", string? keyword = null)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Regular";
        IQueryable<task> query;
        if (userRole == "Admin")
            query = _context.tasks.AsQueryable();
        else
            query = _context.tasks.Where(t => t.userid == userId);
        if (!string.IsNullOrEmpty(status)) query = query.Where(t => t.status == status);
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(t => t.title.Contains(keyword) || t.description.Contains(keyword));
        query = sortBy.ToLower() == "duedate" && sortOrder.ToLower() == "desc"
            ? query.OrderByDescending(t => t.duedate)
            : query.OrderBy(t => t.duedate);
        var tasks = await query.ToListAsync();
        return Ok(tasks);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Regular";
        var task = await _context.tasks.FindAsync(id);
        if (task == null)
            return NotFound();
        if (userRole != "Admin" && task.userid != userId)
            return Unauthorized("You can only edit your own tasks.");
        task.title = dto.Title;
        task.description = dto.Description;
        task.duedate = dto.DueDate;
        task.status = dto.Status;
        task.updatedat = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Regular";
        var task = await _context.tasks.FirstOrDefaultAsync(t => t.id == id);
        if (task == null) return NotFound();
        if (userRole != "Admin" && task.userid != userId)
            return Unauthorized("You can only delete your own tasks.");
        _context.tasks.Remove(task);
        await _context.SaveChangesAsync();
        return Ok("Task deleted.");
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Controllers;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Models;

namespace Tests;

public class TasksControllerTests
{
    private TasksController CreateControllerWithContext(TaskManagementDbContext context, int userId = 1, string role = "Regular")
    {
        var controller = new TasksController(context);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        }, "mock"));

        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        return controller;
    }

    private TaskManagementDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TaskManagementDbContext(options);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnOk_WhenTaskIsCreated()
    {
        var context = CreateInMemoryDbContext();
        var controller = CreateControllerWithContext(context);
        var dto = new TaskDto { Title = "Task 1", Description = "Desc", DueDate = DateTime.Now.AddDays(1), Status = "Pending" };

        var result = await controller.CreateTask(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var task = Assert.IsType<task>(okResult.Value);
        Assert.Equal("Task 1", task.title);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnTasksForRegularUser()
    {
        var context = CreateInMemoryDbContext();
        context.tasks.Add(new task { id = 1, userid = 1, title = "A", description = "", status = "Pending", duedate = DateTime.Now, createdat = DateTime.Now, updatedat = DateTime.Now });
        context.tasks.Add(new task { id = 2, userid = 2, title = "B", description = "", status = "Pending", duedate = DateTime.Now, createdat = DateTime.Now, updatedat = DateTime.Now });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithContext(context, userId: 1);
        var result = await controller.GetTasks(null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var tasks = Assert.IsAssignableFrom<IEnumerable<task>>(okResult.Value);
        Assert.Single(tasks);
        Assert.Equal(1, tasks.First().userid);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnUnauthorized_WhenUserIsNotOwner()
    {
        var context = CreateInMemoryDbContext();
        context.tasks.Add(new task { id = 1, userid = 2, title = "Old", description = "", status = "Pending", duedate = DateTime.Now, createdat = DateTime.Now, updatedat = DateTime.Now });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithContext(context, userId: 1);
        var dto = new TaskDto { Title = "New", Description = "Updated", DueDate = DateTime.Now, Status = "Done" };
        var result = await controller.UpdateTask(1, dto);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("You can only edit your own tasks.", unauthorized.Value);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnOk_WhenAdminDeletesTask()
    {
        var context = CreateInMemoryDbContext();
        context.tasks.Add(new task { id = 1, userid = 99, title = "To Delete", description = "", status = "Pending", duedate = DateTime.Now, createdat = DateTime.Now, updatedat = DateTime.Now });
        await context.SaveChangesAsync();

        var controller = CreateControllerWithContext(context, userId: 1, role: "Admin");
        var result = await controller.DeleteTask(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Task deleted.", ok.Value);
    }
}
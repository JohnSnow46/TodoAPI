using Microsoft.AspNetCore.Mvc;
using TodoAPI.Core.Entities;
using TodoAPI.Core.Interfaces;

namespace TodoAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TestController> _logger;

        public TestController(IUnitOfWork unitOfWork, ILogger<TestController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("test-user")]
        public async Task<ActionResult<User>> CreateTestUser()
        {
            try
            {
                var testUser = new User
                {
                    Email = $"test{DateTime.Now.Ticks}@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    PasswordHash = "test-hash" // In real app, this would be properly hashed
                };

                var createdUser = await _unitOfWork.Users.AddUserAsync(testUser);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("test-task/{userId:guid}")]
        public async Task<ActionResult<TaskItem>> CreateTestTask(Guid userId)
        {
            try
            {
                // Check if user exists
                var userExists = await _unitOfWork.Users.UserExistsAsync(userId);
                if (!userExists)
                {
                    return NotFound("User not found");
                }

                // Get first category for testing
                var categories = await _unitOfWork.Categories.GetAllCategoriesAsync();
                var firstCategory = categories.FirstOrDefault();

                var testTask = new TaskItem
                {
                    Title = $"Test Task {DateTime.Now:yyyy-MM-dd HH:mm}",
                    Description = "This is a test task created via API",
                    Status = Core.Enums.TaskStatus.Todo,
                    Priority = Core.Enums.TaskPriority.Medium,
                    UserId = userId,
                    CategoryId = firstCategory?.Id,
                    User = null!, // Will be loaded by EF
                    Category = null
                };

                var createdTask = await _unitOfWork.Tasks.AddTaskAsync(testTask);
                return Ok(createdTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test task");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tasks")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks()
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("health")]
        public async Task<ActionResult> HealthCheck()
        {
            try
            {
                // Test database connection
                var categoriesCount = (await _unitOfWork.Categories.GetAllCategoriesAsync()).Count();

                return Ok(new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Database = "Connected",
                    CategoriesInDatabase = categoriesCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, new
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }
    }
}
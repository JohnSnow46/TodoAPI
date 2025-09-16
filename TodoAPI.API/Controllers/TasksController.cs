using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.DTOs.Common;
using TodoAPI.Application.DTOs.Task;
using TodoAPI.Core.Entities;
using TodoAPI.Core.Interfaces;

namespace TodoAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TasksController> _logger;

        public TasksController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TasksController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get tasks for current user with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TaskDto>>>> GetTasks([FromQuery] TaskFilterDto filter)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<List<TaskDto>>.ErrorResult("Invalid token"));
                }

                // Get user-specific tasks
                var tasks = await _unitOfWork.Tasks.GetTasksByUserIdAsync(userId.Value);

                // Apply additional filtering if needed
                if (filter.Status.HasValue)
                {
                    tasks = tasks.Where(t => t.Status == filter.Status.Value);
                }

                if (filter.Priority.HasValue)
                {
                    tasks = tasks.Where(t => t.Priority == filter.Priority.Value);
                }

                if (filter.CategoryId.HasValue)
                {
                    tasks = tasks.Where(t => t.CategoryId == filter.CategoryId.Value);
                }

                if (!string.IsNullOrEmpty(filter.Search))
                {
                    tasks = tasks.Where(t => t.Title.Contains(filter.Search, StringComparison.OrdinalIgnoreCase) ||
                                           t.Description.Contains(filter.Search, StringComparison.OrdinalIgnoreCase));
                }

                var taskDtos = _mapper.Map<List<TaskDto>>(tasks.ToList());
                return Ok(ApiResponse<List<TaskDto>>.SuccessResult(taskDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks for user");
                return StatusCode(500, ApiResponse<List<TaskDto>>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Get specific task by ID (only if belongs to current user)
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> GetTask(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<TaskDto>.ErrorResult("Invalid token"));
                }

                var task = await _unitOfWork.Tasks.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(ApiResponse<TaskDto>.ErrorResult("Task not found"));
                }

                // Check if task belongs to current user
                if (task.UserId != userId.Value)
                {
                    return Forbid(); // User can't access other users' tasks
                }

                var taskDto = _mapper.Map<TaskDto>(task);
                return Ok(ApiResponse<TaskDto>.SuccessResult(taskDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task with ID {TaskId}", id);
                return StatusCode(500, ApiResponse<TaskDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Create new task for current user
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskDto>>> CreateTask(CreateTaskDto createTaskDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<TaskDto>.ErrorResult("Invalid token"));
                }

                var task = _mapper.Map<TaskItem>(createTaskDto);
                task.UserId = userId.Value; // Assign to current user

                var createdTask = await _unitOfWork.Tasks.AddTaskAsync(task);
                var taskDto = _mapper.Map<TaskDto>(createdTask);

                return CreatedAtAction(nameof(GetTask), new { id = taskDto.Id },
                    ApiResponse<TaskDto>.SuccessResult(taskDto, "Task created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, ApiResponse<TaskDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Update existing task (only if belongs to current user)
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> UpdateTask(Guid id, UpdateTaskDto updateTaskDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<TaskDto>.ErrorResult("Invalid token"));
                }

                var existingTask = await _unitOfWork.Tasks.GetTaskByIdAsync(id);
                if (existingTask == null)
                {
                    return NotFound(ApiResponse<TaskDto>.ErrorResult("Task not found"));
                }

                // Check if task belongs to current user
                if (existingTask.UserId != userId.Value)
                {
                    return Forbid();
                }

                // Update task properties
                _mapper.Map(updateTaskDto, existingTask);
                var updatedTask = await _unitOfWork.Tasks.UpdateTaskAsync(existingTask);
                var taskDto = _mapper.Map<TaskDto>(updatedTask);

                return Ok(ApiResponse<TaskDto>.SuccessResult(taskDto, "Task updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID {TaskId}", id);
                return StatusCode(500, ApiResponse<TaskDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Update task status only
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> UpdateTaskStatus(Guid id, UpdateTaskStatusDto statusDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<TaskDto>.ErrorResult("Invalid token"));
                }

                var existingTask = await _unitOfWork.Tasks.GetTaskByIdAsync(id);
                if (existingTask == null)
                {
                    return NotFound(ApiResponse<TaskDto>.ErrorResult("Task not found"));
                }

                // Check if task belongs to current user
                if (existingTask.UserId != userId.Value)
                {
                    return Forbid();
                }

                existingTask.Status = statusDto.Status;
                var updatedTask = await _unitOfWork.Tasks.UpdateTaskAsync(existingTask);
                var taskDto = _mapper.Map<TaskDto>(updatedTask);

                return Ok(ApiResponse<TaskDto>.SuccessResult(taskDto, "Task status updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status for ID {TaskId}", id);
                return StatusCode(500, ApiResponse<TaskDto>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Delete task (only if belongs to current user)
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTask(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<bool>.ErrorResult("Invalid token"));
                }

                var task = await _unitOfWork.Tasks.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResult("Task not found"));
                }

                // Check if task belongs to current user
                if (task.UserId != userId.Value)
                {
                    return Forbid();
                }

                var deleted = await _unitOfWork.Tasks.DeleteTaskAsync(id);
                if (deleted)
                {
                    return Ok(ApiResponse<bool>.SuccessResult(true, "Task deleted successfully"));
                }

                return BadRequest(ApiResponse<bool>.ErrorResult("Failed to delete task"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID {TaskId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Get task statistics for current user
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<object>>> GetTaskStats()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Invalid token"));
                }

                var totalTasks = await _unitOfWork.Tasks.GetTaskCountByUserAsync(userId.Value);
                var completedTasks = await _unitOfWork.Tasks.GetCompletedTaskCountByUserAsync(userId.Value);
                var pendingTasks = totalTasks - completedTasks;

                var stats = new
                {
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    PendingTasks = pendingTasks,
                    CompletionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0
                };

                return Ok(ApiResponse<object>.SuccessResult(stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task statistics");
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error"));
            }
        }

        /// <summary>
        /// Helper method to get current user ID from JWT token
        /// </summary>
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return null;
        }
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.DTOs.Common;
using TodoAPI.Application.DTOs.Task;
using TodoAPI.Core.Entities;
using TodoAPI.Core.Interfaces;

namespace TodoAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TaskDto>>>> GetTasks([FromQuery] TaskFilterDto filter)
        {
            try
            {
                // For now, get all tasks (later we'll add filtering and user-specific tasks)
                var tasks = await _unitOfWork.Tasks.GetAllTasksAsync();
                var taskDtos = _mapper.Map<List<TaskDto>>(tasks);

                return Ok(ApiResponse<List<TaskDto>>.SuccessResult(taskDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(500, ApiResponse<List<TaskDto>>.ErrorResult("Internal server error"));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> GetTask(Guid id)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(ApiResponse<TaskDto>.ErrorResult("Task not found"));
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

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskDto>>> CreateTask(CreateTaskDto createTaskDto)
        {
            try
            {
                // TODO: Get current user ID from JWT token
                // For now, we'll need to pass userId or use the first user
                var users = await _unitOfWork.Users.GetAllUsersAsync();
                var firstUser = users.FirstOrDefault();

                if (firstUser == null)
                {
                    return BadRequest(ApiResponse<TaskDto>.ErrorResult("No users found. Please create a user first."));
                }

                var task = _mapper.Map<TaskItem>(createTaskDto);
                task.UserId = firstUser.Id; // TODO: Replace with current user ID from JWT

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

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTask(Guid id)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResult("Task not found"));
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
    }
}
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.DTOs.Category;
using TodoAPI.Application.DTOs.Common;
using TodoAPI.Core.Entities;
using TodoAPI.Core.Interfaces;

namespace TodoAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoriesController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllCategoriesAsync();
                var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

                return Ok(ApiResponse<List<CategoryDto>>.SuccessResult(categoryDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, ApiResponse<List<CategoryDto>>.ErrorResult("Internal server error"));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(Guid id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound(ApiResponse<CategoryDto>.ErrorResult("Category not found"));
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Ok(ApiResponse<CategoryDto>.SuccessResult(categoryDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID {CategoryId}", id);
                return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult("Internal server error"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            try
            {
                // Check if category with same name exists
                var existingCategory = await _unitOfWork.Categories.GetCategoryByNameAsync(createCategoryDto.Name);
                if (existingCategory != null)
                {
                    return BadRequest(ApiResponse<CategoryDto>.ErrorResult("Category with this name already exists"));
                }

                var category = _mapper.Map<Category>(createCategoryDto);
                var createdCategory = await _unitOfWork.Categories.AddCategoryAsync(category);
                var categoryDto = _mapper.Map<CategoryDto>(createdCategory);

                return CreatedAtAction(nameof(GetCategory), new { id = categoryDto.Id },
                    ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Category created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult("Internal server error"));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(Guid id, UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                var existingCategory = await _unitOfWork.Categories.GetCategoryByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(ApiResponse<CategoryDto>.ErrorResult("Category not found"));
                }

                // Check if another category with same name exists
                var categoryWithSameName = await _unitOfWork.Categories.GetCategoryByNameAsync(updateCategoryDto.Name);
                if (categoryWithSameName != null && categoryWithSameName.Id != id)
                {
                    return BadRequest(ApiResponse<CategoryDto>.ErrorResult("Category with this name already exists"));
                }

                _mapper.Map(updateCategoryDto, existingCategory);
                var updatedCategory = await _unitOfWork.Categories.UpdateCategoryAsync(existingCategory);
                var categoryDto = _mapper.Map<CategoryDto>(updatedCategory);

                return Ok(ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Category updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {CategoryId}", id);
                return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult("Internal server error"));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(Guid id)
        {
            try
            {
                var categoryExists = await _unitOfWork.Categories.CategoryExistsAsync(id);
                if (!categoryExists)
                {
                    return NotFound(ApiResponse<bool>.ErrorResult("Category not found"));
                }

                var deleted = await _unitOfWork.Categories.DeleteCategoryAsync(id);
                if (deleted)
                {
                    return Ok(ApiResponse<bool>.SuccessResult(true, "Category deleted successfully"));
                }

                return BadRequest(ApiResponse<bool>.ErrorResult("Failed to delete category"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error"));
            }
        }
    }
}
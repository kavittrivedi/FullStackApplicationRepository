using ApiApplicationCore.Dtos;
using ApiApplicationCore.Models;
using ApiApplicationCore.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplicationCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;

        }

        [HttpGet("GetAllCategories")]
        public IActionResult GetAllCategories()
        {
            var response = _categoryService.GetCategories();
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("GetCategoryById/{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var response = _categoryService.GetCategory(id);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("Create")]
        public IActionResult AddCategory(AddCategoryDto categoryDto)
        {

            var category = new Category()
            {
                Name = categoryDto.CategoryName,
                Description = categoryDto.CategoryDescription,
            };

            var result = _categoryService.AddCategory(category);
            return !result.Success ? BadRequest(result) : Ok(result);

        }

        [HttpPut("ModifyCategory")]
        public IActionResult UpdateCategory(UpdateCategoryDto categoryDto)
        {
            var category = new Category()
            {
                CategoryId = categoryDto.CategoryId,
                Name = categoryDto.CategoryName,
                Description = categoryDto.CategoryDescription,
            };

            var response = _categoryService.ModifyCategory(category);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpDelete("Remove/{id}")]
        public IActionResult RemoveCategory(int id)
        {
            if (id > 0)
            {
                var response = _categoryService.RemoveCategory(id);

                if (!response.Success)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            else
            {
                return BadRequest("Please enter proper data.");
            }
        }
    }
}

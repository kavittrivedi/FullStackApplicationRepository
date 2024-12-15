using ApiApplicationCore.Data.Contract;
using ApiApplicationCore.Dtos;
using ApiApplicationCore.Models;
using ApiApplicationCore.Services.Contract;

namespace ApiApplicationCore.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public ServiceResponse<IEnumerable<CategoryDto>> GetCategories()
        {
            var response = new ServiceResponse<IEnumerable<CategoryDto>>();
            var categories = _categoryRepository.GetAll();
            if (categories != null && categories.Any())
            {
                categories.Where(c => c.FileName == string.Empty).ToList();
                List<CategoryDto> categoryDtos = new List<CategoryDto>();
                foreach (var category in categories)
                {
                    categoryDtos.Add(
                        new CategoryDto() { CategoryId = category.CategoryId, CategoryName = category.Name, CategoryDescription = category.Description }
                        );
                }

                response.Data = categoryDtos;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found!";
            }

            return response;
        }

        public ServiceResponse<CategoryDto> GetCategory(int categoryId)
        {
            var response = new ServiceResponse<CategoryDto>();
            var existingCategory = _categoryRepository.GetCategory(categoryId);
            if (existingCategory != null)
            {
                var category = new CategoryDto()
                {
                    CategoryId = categoryId,
                    CategoryName = existingCategory.Name,
                    CategoryDescription = existingCategory.Description
                };

                response.Data = category;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found!";
            }

            return response;
        }

        public ServiceResponse<string> AddCategory(Category category)
        {
            var response = new ServiceResponse<string>();
            if (_categoryRepository.CategoryExists(category.Name))
            {
                response.Success = false;
                response.Message = "Category already exists.";
                return response;
            }

            var fileName = string.Empty;
            category.FileName = fileName;

            var result = _categoryRepository.InsertCategory(category);
            if (result)
            {
                response.Message = "Category saved successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime.";
            }

            return response;
        }

        public ServiceResponse<string> ModifyCategory(Category category)
        {
            var response = new ServiceResponse<string>();

            if (_categoryRepository.CategoryExists(category.CategoryId, category.Name))
            {
                response.Success = false;
                response.Message = "Category already exists.";
                return response;
            }

            var existingCategory = _categoryRepository.GetCategory(category.CategoryId);
            var result = false;
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                result = _categoryRepository.UpdateCategory(existingCategory);
            }

            if (result)
            {
                response.Message = "Category updated successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime.";
            }

            return response;
        }

        public ServiceResponse<string> RemoveCategory(int id)
        {
            var response = new ServiceResponse<string>();
            var result = _categoryRepository.DeleteCategory(id);
            if (result)
            {
                response.Message = "Category deleted successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometimes.";
            }

            return response;
        }
    }
}

using ApiApplicationCore.Dtos;
using ApiApplicationCore.Models;

namespace ApiApplicationCore.Services.Contract
{
    public interface ICategoryService
    {
        ServiceResponse<IEnumerable<CategoryDto>> GetCategories();

        ServiceResponse<CategoryDto> GetCategory(int categoryId);

        ServiceResponse<string> AddCategory(Category category);

        ServiceResponse<string> ModifyCategory(Category category);

        ServiceResponse<string> RemoveCategory(int id);
    }
}

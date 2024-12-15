using MVCApplicationCore.Models;

namespace MVCApplicationCore.Services.Contract
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();

        int TotalCategories();

        IEnumerable<Category> GetPaginatedCategories(int page, int pageSize);

        Category? GetCategory(int id);

        string AddCategory(Category category, IFormFile file);

        string RemoveCategory(int id);

        string ModifyCategory(Category category);
    }
}

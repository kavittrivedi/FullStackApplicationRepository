using ApiApplicationCore.Models;

namespace ApiApplicationCore.Data.Contract
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();

        int TotalCategories();

        IEnumerable<Category> GetPaginatedCategories(int page, int pageSize);

        Category? GetCategory(int id);

        bool CategoryExists(string name);

        bool CategoryExists(int categoryId, string name);

        bool InsertCategory(Category category);

        bool UpdateCategory(Category category);

        bool DeleteCategory(int id);
    }
}

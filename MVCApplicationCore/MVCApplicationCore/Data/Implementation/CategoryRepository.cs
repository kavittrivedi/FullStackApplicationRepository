using Microsoft.EntityFrameworkCore;
using MVCApplicationCore.Data.Contract;
using MVCApplicationCore.Models;
using NuGet.Protocol;

namespace MVCApplicationCore.Data.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IAppDbContext _appDbContext;

        public CategoryRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Category> GetAll()
        {
            List<Category> categories = _appDbContext.Categories.ToList();
            return categories;
        }

        public int TotalCategories()
        {
            return _appDbContext.Categories.Count();
        }

        public IEnumerable<Category> GetPaginatedCategories(int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            return _appDbContext.Categories
                .OrderBy(c => c.CategoryId)
                .Skip(skip)
                .Take(pageSize)
                .ToList();
        }

        public Category? GetCategory(int id)
        {
            var category = _appDbContext.Categories.FirstOrDefault(c => c.CategoryId == id);
            return category;
        }

        public bool InsertCategory(Category category)
        {
            var result = false;
            if (category != null)
            {
                _appDbContext.Categories.Add(category);
                _appDbContext.SaveChanges();
                result = true;
            }

            return result;
        }

        public bool UpdateCategory(Category category)
        {
            var result = false;
            if (category != null)
            {
                _appDbContext.Categories.Update(category);
                //_appDbContext.Entry(category).State = EntityState.Modified;
                _appDbContext.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool DeleteCategory(int id)
        {
            var result = false;
            var category = _appDbContext.Categories.Find(id);
            if (category != null)
            {
                _appDbContext.Categories.Remove(category);
                _appDbContext.SaveChanges();
                result = true;
            }

            return result;
        }

        public bool CategoryExists(string name)
        {
            var category = _appDbContext.Categories.FirstOrDefault(c => c.Name == name);
            if (category != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CategoryExists(int categoryId, string name)
        {
            var category = _appDbContext.Categories.FirstOrDefault(c => c.CategoryId != categoryId && c.Name == name);
            if (category != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<CategoryProductCount> GetCategoryProductCounts()
        {
            return  _appDbContext.GetCategoryProductCounts();
        }
    }
}

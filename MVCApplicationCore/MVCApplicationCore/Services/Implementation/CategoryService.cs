using MVCApplicationCore.Data.Contract;
using MVCApplicationCore.Models;
using MVCApplicationCore.Services.Contract;

namespace MVCApplicationCore.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IEnumerable<Category> GetCategories()
        {
            var categories = _categoryRepository.GetAll();
            if (categories != null && categories.Any())
            {
                //foreach (var category in categories.Where(c => c.FileName == string.Empty))
                //{
                //    category.FileName = "DefaultImage.png";
                //}
                categories.Where(c => c.FileName == string.Empty).ToList() // To prevent modification of the collection while iterating
                .ForEach(category => category.FileName = "DefaultImage.png");

                return categories;
            }

            return new List<Category>();
        }

        public int TotalCategories()
        {
            return _categoryRepository.TotalCategories();
        }

        public IEnumerable<Category> GetPaginatedCategories(int page, int pageSize)
        {
            return _categoryRepository.GetPaginatedCategories(page, pageSize);
        }

        public Category? GetCategory(int id)
        {
            var category = _categoryRepository.GetCategory(id);
            return category;
        }

        public string AddCategory(Category category, IFormFile file)
        {
            if (_categoryRepository.CategoryExists(category.Name))
            {
                return "Category already exists.";
            }

            var fileName = string.Empty;
            if (file != null && file.Length > 0)
            {
                // Process the uploaded file(eg. save it to disk)
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file.FileName);

                // Save the file to storage and set path.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    fileName = file.FileName;
                }

                category.FileName = fileName;
            }


            var result = _categoryRepository.InsertCategory(category);

            return result ? "Category saved successfully." : "Something went wrong, please try after sometime.";
        }

        public string ModifyCategory(Category category)
        {
            var message = string.Empty;
            if (_categoryRepository.CategoryExists(category.CategoryId, category.Name))
            {
                message = "Category already exists.";
                return message;
            }

            var existingCategory = _categoryRepository.GetCategory(category.CategoryId);
            var result = false;
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                result = _categoryRepository.UpdateCategory(existingCategory);
            }

            message = result ? "Category updated successfully." : "Something went wrong, please try after sometime.";
            return message;
        }

        public string RemoveCategory(int id)
        {
            var result = _categoryRepository.DeleteCategory(id);
            if (result)
            {
                return "Category deleted successfully.";
            }
            else
            {
                return "Something went wrong, please try after sometimes.";
            }
        }
    }
}

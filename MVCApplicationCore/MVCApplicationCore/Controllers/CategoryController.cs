using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCApplicationCore.Models;
using MVCApplicationCore.Services.Contract;
using MVCApplicationCore.ViewModels;

namespace MVCApplicationCore.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {

            _categoryService = categoryService;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            // Calculate the number of items to skip based on the page number and page size
            int skip = (page - 1) * pageSize;
            var categories = _categoryService.GetCategories();
            if (categories != null && categories.Any())
            {
                return View(categories);
            }

            return View(new List<Category>());
        }

        public IActionResult Index1()
        {
            var categories = _categoryService.GetCategories();
            if (categories != null && categories.Any())
            {
                return View("CategoryList", categories);
            }

            return View("CategoryList", new List<Category>());
        }

        public IActionResult Index2(int page = 1, int pageSize = 2)
        {
            ViewBag.CurrentPage = page; // Pass the current page number to the ViewBag
            // Get total count of categories
            var totalCount = _categoryService.TotalCategories();

            // Calculate total number of pages
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            // Get paginated categories
            var categories = _categoryService.GetPaginatedCategories(page, pageSize);

            // Set ViewBag properties
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(categories);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _categoryService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var message = _categoryService.ModifyCategory(category);
                if (message == "Category already exists." || message == "Something went wrong, please try after sometime.")
                {
                    TempData["ErrorMessage"] = message;
                }
                else
                {
                    TempData["SuccessMessage"] = message;
                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category()
                {
                    Name = categoryViewModel.Name,
                    Description = categoryViewModel.Description,
                };

                var result = _categoryService.AddCategory(category, categoryViewModel.File);
                if (result == "Category already exists." || result == "Something went wrong, please try after sometime.")
                {
                    TempData["ErrorMessage"] = result;
                }
                else if (result == "Category saved successfully.")
                {
                    TempData["SuccessMessage"] = result;
                    return RedirectToAction("Index");
                }
            }

            return View(categoryViewModel);
        }

        public IActionResult Details(int id)
        {
            var category = _categoryService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = _categoryService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int categoryId)
        {
            var result = _categoryService.RemoveCategory(categoryId);

            if (result == "Category deleted successfully.")
            {
                TempData["SuccessMessage"] = result;
            }
            else
            {
                TempData["ErrorMessage"] = result;
            }

            return RedirectToAction("Index");
        }
    }
}

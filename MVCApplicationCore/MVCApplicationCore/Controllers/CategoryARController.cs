using Microsoft.AspNetCore.Mvc;
using MVCApplicationCore.Models;

namespace MVCApplicationCore.Controllers
{
    [Route("categories")]
    public class CategoryARController : Controller
    {
        private static List<Category> _categories = new List<Category>
        {
            new Category{ CategoryId=1, Name="Category 1", Description="Description 1" },
            new Category{ CategoryId=2, Name="Category 2", Description="Description 2" },
        };

        [HttpGet("")]
        public IActionResult Index()
        {
            return View(_categories);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            Category category = new Category();
            return View(category);
        }

        [HttpPost("create")]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CategoryId = _categories.Count() + 1;
                _categories.Add(category);
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var category = _categories.Find(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = _categories.Find(c => c.CategoryId == id);
                if (existingCategory != null)
                {
                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    return RedirectToAction("Index");
                }
            }

            return View(category);
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            var category = _categories.Find(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var category = _categories.Find(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost("delete/{id}")]
        public IActionResult DeleteConfirmed(int categoryId)
        {
            var category = _categories.Find(c => c.CategoryId == categoryId);
            if (category != null)
            {
                _categories.Remove(category);
            }

            return RedirectToAction("Index");
        }
    }
}

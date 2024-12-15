using Microsoft.AspNetCore.Mvc;
using MVCApplicationCore.Models;

namespace MVCApplicationCore.Controllers
{
    public class CategoryMVC5Controller : Controller
    {
        private static List<Category> _categories = new List<Category>
        {
            new Category{ CategoryId=1, Name="Category 1", Description="Description 1" },
            new Category{ CategoryId=2, Name="Category 2", Description="Description 2" },
        };
        public IActionResult Index()
        {
            return View(_categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
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

        public IActionResult Edit(int id)
        {
            var category = _categories.Find(c => c.CategoryId == id);
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
                var existingCategory = _categories.Find(c => c.CategoryId == category.CategoryId);
                if (existingCategory != null)
                {
                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    return RedirectToAction("Index");
                }
            }

            return View(category);

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _categories.Find(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int categoryId)
        {
            var category = _categories.Find(c => c.CategoryId == categoryId);
            if (category != null)
            {
                _categories.Remove(category);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var category = _categories.Find(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MVCApplicationCore.Models;
using MVCApplicationCore.Services.Contract;
using MVCApplicationCore.ViewModels;

namespace MVCApplicationCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Categories = new List<Models.Category>();
            productViewModel.Categories = _categoryService.GetCategories().ToList();
            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult Create(ProductViewModel productviewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    CategoryId = productviewModel.CategoryId,
                    ProductName = productviewModel.ProductName,
                    ProductDescription = productviewModel.ProductDescription,
                    ProductPrice = productviewModel.ProductPrice,
                    InStock = productviewModel.InStock,
                    IsActive = productviewModel.IsActive,
                };

                var message = _productService.AddProduct(product);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (message == "Something went wrong. Please try after sometime." || message == "Product already exists.")
                    {
                        TempData["ErrorMessage"] = message;
                    }
                    else
                    {
                        TempData["SuccessMessage"] = message;
                        return RedirectToAction("Index");
                    }
                }
            }
            productviewModel.Categories = new List<Models.Category>();
            productviewModel.Categories = _categoryService.GetCategories().ToList();
            return View(productviewModel);
        }
    }
}

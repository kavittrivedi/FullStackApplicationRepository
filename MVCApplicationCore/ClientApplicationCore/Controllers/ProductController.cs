using ClientApplicationCore.Infrastructure;
using ClientApplicationCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClientApplicationCore.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private string endPoint;

        public ProductController(IHttpClientService httpClientService, IConfiguration configuration)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }

        [HttpGet]
        public IActionResult Index()
        {
            ServiceResponse<IEnumerable<ProductsViewModel>> response = new ServiceResponse<IEnumerable<ProductsViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductsViewModel>>>
                ($"{endPoint}Product/GetAllProducts", HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                return View(response.Data);
            }

            return View(new List<ProductsViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Categories = GetCategories();
            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = $"{endPoint}Product/AddProduct";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong. Please try after sometime.";
                    }
                }

                return RedirectToAction("Index");
            }


            viewModel.Categories = GetCategories();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateMVC5()
        {
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Categories = GetCategories();
            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMVC5(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = $"{endPoint}Product/AddProduct";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong. Please try after sometime.";
                    }
                }

                return RedirectToAction("Index");
            }


            viewModel.Categories = GetCategories();
            return View(viewModel);
        }
        

        private List<CategoryViewModel> GetCategories()
        {
            ServiceResponse<IEnumerable<CategoryViewModel>> response = new ServiceResponse<IEnumerable<CategoryViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<CategoryViewModel>>>
                ($"{endPoint}Category/GetAllCategories", HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                return response.Data.ToList();
            }

            return new List<CategoryViewModel>();
        }
    }
}

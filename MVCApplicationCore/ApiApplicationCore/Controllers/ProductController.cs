using ApiApplicationCore.Dtos;
using ApiApplicationCore.Models;
using ApiApplicationCore.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplicationCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        { 
            var response = _productService.GetAllProducts();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("AddProduct")]
        public IActionResult AddProduct(AddProductDto addProduct)
        {
            var product = new Product
            {
                CategoryId = addProduct.CategoryId,
                ProductName = addProduct.ProductName,   
                ProductDescription= addProduct.ProductDescription,
                ProductPrice = addProduct.ProductPrice,
                InStock = addProduct.InStock,
                IsActive = addProduct.IsActive,
            };

            var result = _productService.AddProduct(product);
            return !result.Success ? BadRequest(result) : Ok(result);
        }
        
    }
}

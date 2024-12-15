using ApiApplicationCore.Data.Contract;
using ApiApplicationCore.Dtos;
using ApiApplicationCore.Models;
using ApiApplicationCore.Services.Contract;

namespace ApiApplicationCore.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ServiceResponse<string> AddProduct(Product product)
        {
            var response = new ServiceResponse<string>();

            if (product == null)
            {
                response.Success = false;
                response.Message = "Something went wrong. Please try after sometime.";
                return response;
            }
            if (AlreadyExists(product.CategoryId, product.ProductName))
            {
                response.Success = false;
                response.Message = "Product already exists.";
                return response;
            }

            var result = _productRepository.InsertProduct(product);
            response.Success = result;
            response.Message = result ? "Product saved successfully." : "Something went wrong. Please try after sometime.";

            return response;
        }

        public ServiceResponse<string> UpdateProduct(Product product)
        {
            var response = new ServiceResponse<string>();
            if (product == null)
            {
                response.Success = false;
                response.Message = "Something went wrong. Please try after sometime.";
                return response;
            }
            if (AlreadyExists(product.ProductId, product.CategoryId, product.ProductName))
            {
                response.Success = false;
                response.Message = "Product already exists.";
                return response;
            }


            var updatedProduct = _productRepository.GetProductById(product.ProductId);
            if (updatedProduct != null)
            {
                updatedProduct.ProductName = product.ProductName;
                updatedProduct.ProductDescription = product.ProductDescription;
                updatedProduct.ProductPrice = product.ProductPrice;
                updatedProduct.CategoryId = product.CategoryId;
                updatedProduct.InStock = product.InStock;
                updatedProduct.IsActive = product.IsActive;
                var result = _productRepository.UpdateProduct(updatedProduct);

                response.Success = result;
                response.Message = result ? "Product updated successfully." : "Something went wrong. Please try after sometime.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong. Please try after sometime.";
                return response;
            }

            return response;
        }

        public ServiceResponse<string> DeleteProduct(int id)
        {
            var response = new ServiceResponse<string>();

            if (id < 0)
            {
                response.Success = false;
                response.Message = "No record to delete.";

            }

            var result = _productRepository.DeleteProduct(id);
            response.Success = result;
            response.Message = result ? "Product deleted successfully." : "Something went wrong, please try after sometime.";

            return response;
        }

        public ServiceResponse<IEnumerable<ProductDto>> GetAllProducts()
        {
            var response = new ServiceResponse<IEnumerable<ProductDto>>();
            var products = _productRepository.GetAllProducts();

            if (products == null || !products.Any())
            {
                response.Success = false;
                response.Data = new List<ProductDto>();
                response.Message = "No record found.";
                return response;
            }

            List<ProductDto> productDtos = new List<ProductDto>();
            foreach (var product in products.ToList())
            {
                productDtos.Add(
                    new ProductDto()
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductDescription = product.ProductDescription,
                        ProductPrice = product.ProductPrice,
                        CategoryId = product.CategoryId,
                        InStock = product.InStock,
                        IsActive = product.IsActive,
                        Category = new Category()
                        {
                            CategoryId = product.Category.CategoryId,
                            Name = product.Category.Name,
                        },
                    });

            }

            response.Data = productDtos;
            return response;
        }

        public ServiceResponse<ProductDto> GetProductById(int id)
        {
            var response = new ServiceResponse<ProductDto>();

            var product = _productRepository.GetProductById(id);

            var productDto = new ProductDto()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                CategoryId = product.CategoryId,
                InStock = product.InStock,
                IsActive = product.IsActive,
                Category = new Category()
                {
                    CategoryId = product.Category.CategoryId,
                    Name = product.Category.Name,
                },
            };

            response.Data = productDto;
            return response;
        }

        private bool AlreadyExists(int categoryId, string productName)
        {
            var result = false;
            var product = _productRepository.GetProductByCategoryIdAndProductName(categoryId, productName);
            if (product != null)
            {
                result = true;
            }

            return result;
        }

        private bool AlreadyExists(int productId, int categoryId, string productName)
        {
            var result = false;
            var product = _productRepository
                .GetProductByCategoryIdAndProductName(productId,
                categoryId, productName);
            if (product != null)
            {
                result = true;
            }

            return result;
        }
    }
}

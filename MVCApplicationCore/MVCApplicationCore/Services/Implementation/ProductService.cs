using MVCApplicationCore.Data.Contract;
using MVCApplicationCore.Models;
using MVCApplicationCore.Services.Contract;

namespace MVCApplicationCore.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public string AddProduct(Product product)
        {
            var message = string.Empty;
            if (product == null)
            {
                message = "Something went wrong. Please try after sometime.";
            }
            else if (AlreadyExists(product.CategoryId, product.ProductName))
            {
                message = "Product already exists.";
            }
            else
            {
                var result = _productRepository.InsertProduct(product);
                message = result ? "Product saved successfully." : "Something went wrong. Please try after sometime.";
            }

            return message;
        }

        public string UpdateProduct(Product product)
        {
            var message = string.Empty;
            if (product == null)
            {
                message = "Something went wrong. Please try after sometime.";
            }
            else if (AlreadyExists(product.ProductId, product.CategoryId, product.ProductName))
            {
                message = "Product already exists.";
            }
            else
            {
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
                    message = result ? "Product updated successfully." : "Something went wrong. Please try after sometime.";
                }
            }

            return message;
        }

        public string DeleteProduct(int id)
        {
            var message = string.Empty;
            if (id > 0)
            {
                var result = _productRepository.DeleteProduct(id);
                message = result ? "Product deleted successfully." : "Something went wrong, please try after sometime.";
            }
            else
            {
                message = "No record to delete.";
            }

            return message;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var products = _productRepository.GetAllProducts();

            if (products == null)
            {
                products = new List<Product>();
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            var product = _productRepository.GetProductById(id);
            return product;
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

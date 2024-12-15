using ApiApplicationCore.Dtos;
using ApiApplicationCore.Models;

namespace ApiApplicationCore.Services.Contract
{
    public interface IProductService
    {
        ServiceResponse<string> AddProduct(Product product);

        ServiceResponse<string> UpdateProduct(Product product);

        ServiceResponse<string> DeleteProduct(int id);

        ServiceResponse<IEnumerable<ProductDto>> GetAllProducts();

        ServiceResponse<ProductDto> GetProductById(int id);
    }
}

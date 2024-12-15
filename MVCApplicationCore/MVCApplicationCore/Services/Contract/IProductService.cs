using MVCApplicationCore.Models;

namespace MVCApplicationCore.Services.Contract
{
    public interface IProductService
    {
        string AddProduct(Product product);

        string UpdateProduct(Product product);

        string DeleteProduct(int id);

        IEnumerable<Product> GetAllProducts();

        Product GetProductById(int id);
    }
}

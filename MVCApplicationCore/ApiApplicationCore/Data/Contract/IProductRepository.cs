using ApiApplicationCore.Models;

namespace ApiApplicationCore.Data.Contract
{
    public interface IProductRepository
    {
        bool InsertProduct(Product product);

        IEnumerable<Product> GetAllProducts();

        Product GetProductById(int id);

        bool UpdateProduct(Product product);

        bool DeleteProduct(int id);

        Product GetProductByCategoryIdAndProductName(int categoryId, string productName);

        Product GetProductByCategoryIdAndProductName(int productId, int categoryId, string productName);
    }
}

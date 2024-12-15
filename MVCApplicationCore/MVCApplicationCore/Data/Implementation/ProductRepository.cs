using Microsoft.EntityFrameworkCore;
using MVCApplicationCore.Data.Contract;
using MVCApplicationCore.Models;

namespace MVCApplicationCore.Data.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly IAppDbContext _context;

        public ProductRepository(IAppDbContext context)
        {
            _context = context;
        }

        public bool InsertProduct(Product product)
        {
            var result = false;
            if (product != null)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                result = true;
            }

            return result;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var products = new List<Product>();
            products = _context.Products.Include(p => p.Category).ToList();
            return products;
        }

        public Product GetProductById(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(c => c.ProductId == id);
            return product;
        }

        public bool UpdateProduct(Product product)
        {
            var result = false;
            if (product != null)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                result = true;
            }

            return result;
        }

        public bool DeleteProduct(int id)
        {
            var result = false;
            if (id > 0)
            {
                var product = _context.Products.Find(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                    result = true;
                }
            }

            return result;
        }

        public Product GetProductByCategoryIdAndProductName(int categoryId, string productName)
        {
            var product = _context.Products.FirstOrDefault(c => c.CategoryId == categoryId && c.ProductName == productName);
            return product;
        }

        public Product GetProductByCategoryIdAndProductName(int productId, int categoryId, string productName)
        {
            var product = _context.Products.FirstOrDefault(c => c.ProductId != productId 
            && c.CategoryId == categoryId && c.ProductName == productName);
            return product;
        }
    }
}

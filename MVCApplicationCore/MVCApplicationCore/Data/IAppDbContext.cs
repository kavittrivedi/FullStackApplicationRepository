using Microsoft.EntityFrameworkCore;
using MVCApplicationCore.Models;

namespace MVCApplicationCore.Data
{
    public interface IAppDbContext: IDbContext
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<CategoryProductCount> CategoryProductCounts { get; set; }

        List<CategoryProductCount> GetCategoryProductCounts();

    }
}

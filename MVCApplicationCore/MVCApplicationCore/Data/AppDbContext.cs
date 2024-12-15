using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MVCApplicationCore.Models;

namespace MVCApplicationCore.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // Keyless entity configuration
        public DbSet<CategoryProductCount> CategoryProductCounts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }

        public EntityState GetEntryState<TEntity>(TEntity entity) where TEntity : class
        {
            return Entry(entity).State;
        }

        public void SetEntryState<TEntity>(TEntity entity, EntityState entityState) where TEntity : class
        {
            Entry(entity).State = entityState;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.CategoryId);

            // Ignore the CategoryProductCount model
            //modelBuilder.Ignore<CategoryProductCount>();
            // Ignore CategoryProductCount to avoid creating a table for it
            modelBuilder.Entity<CategoryProductCount>().HasNoKey();

        }

        public List<CategoryProductCount> GetCategoryProductCounts()
        {
            // Using FromSqlRaw to call stored procedure
            return this.Set<CategoryProductCount>().FromSqlRaw("EXEC GetCategoryProductCount").ToList();
        }
    }
}

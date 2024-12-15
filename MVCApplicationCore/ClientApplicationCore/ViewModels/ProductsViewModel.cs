using System.ComponentModel.DataAnnotations;

namespace ClientApplicationCore.ViewModels
{
    public class ProductsViewModel
    {

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public int CategoryId { get; set; }

        public decimal ProductPrice { get; set; }

        public ProductCategoryViewModel Category { get; set; }

        public bool InStock { get; set; }

        public bool IsActive { get; set; }
    }
}

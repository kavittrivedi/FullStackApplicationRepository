using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ClientApplicationCore.ViewModels
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [DisplayName("Product name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product description is required.")]
        [DisplayName("Product description")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Product price is required.")]
        [DisplayName("Product price")]
        public decimal ProductPrice { get; set; }

        [DisplayName("Category")]
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        public List<CategoryViewModel>? Categories { get; set; }

        [DisplayName("Available in stock")]
        public bool InStock { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
    }
}

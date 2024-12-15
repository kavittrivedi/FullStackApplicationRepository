using System.ComponentModel.DataAnnotations;

namespace ApiApplicationCore.Dtos
{
    public class AddProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(50)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product description is required.")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Product category is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Product price is required.")]
        public decimal ProductPrice { get; set; }

        [Required]
        public bool InStock { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}

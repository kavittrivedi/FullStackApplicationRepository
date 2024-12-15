using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MVCApplicationCore.ViewModels
{
    public class CategoryViewModel
    {
        [DisplayName("Category id")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Name is required.")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; }
    }
}

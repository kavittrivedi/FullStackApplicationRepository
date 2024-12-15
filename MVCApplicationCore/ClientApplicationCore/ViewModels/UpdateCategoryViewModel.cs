using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClientApplicationCore.ViewModels
{
    public class UpdateCategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Category name is required.")]
        [DisplayName("Category name")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Category description is required.")]
        [DisplayName("Category description")]
        public string CategoryDescription { get; set; }
    }
}

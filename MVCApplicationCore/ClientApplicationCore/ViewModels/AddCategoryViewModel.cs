using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ClientApplicationCore.ViewModels
{
    public class AddCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [DisplayName("Category name")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Category description is required.")]
        [DisplayName("Category description")]
        public string CategoryDescription { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Areas.Admin.Models.ViewModel
{
    public class CategoryViewModel
    {
        [Required(ErrorMessage ="Name Category is required!")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Description Category is required!")]
        public string Description { get; set; }
    }
}

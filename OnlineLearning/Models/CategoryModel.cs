using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class CategoryModel
    {
        [Key] 
        public int CategoryId { get; set; }
        public string Fullname { get; set; }
        public string description { get; set; }
    }
}

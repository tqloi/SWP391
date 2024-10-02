using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryID { get; set; }

        [MaxLength(50)]
        public string FullName { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; }
    }
}

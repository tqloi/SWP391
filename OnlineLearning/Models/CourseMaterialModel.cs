using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class CourseMaterialModel
    {
        [Key]
        public int MaterialID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public string FIleName { get; set; }
        public string fileExtension { get; set; }

        [Required]
        [StringLength(255)]
        public string MaterialsLink { get; set; }

        public virtual CourseModel Course { get; set; }
    }
}

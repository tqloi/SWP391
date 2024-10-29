using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class BookMarkModel
    {
        [Key]
        public int BookmarkID { get; set; }

        [ForeignKey("Student")]
        [StringLength(450)]
        public string StudentID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }

        // Navigation properties
        public AppUserModel Student { get; set; }
        public  CourseModel Course { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class BookMarkModel
    {
        [Key]
        public int BookmarkID { get; set; }

        [Required]
        [StringLength(450)]
        public string StudentID { get; set; } // AspNetUsers Foreign Key

        [Required]
        public int CourseID { get; set; } // Courses Foreign Key

        // Navigation properties
        [ForeignKey("StudentID")]
        public virtual AppUserModel Student { get; set; } // Assuming your user model is called ApplicationUser

        [ForeignKey("CourseID")]
        public virtual CourseModel Course { get; set; } // Assuming your Course model is called Course

    }
}

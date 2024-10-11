using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class LectureModel
    {
        [Key]
        public int LectureID { get; set; }  

        [ForeignKey("Course")]  
        public int CourseID { get; set; }

        [Required]  
        [StringLength(255)] 
        public string Title { get; set; }

        [StringLength(255)]  
        public string Description { get; set; }

        [DataType(DataType.DateTime)]  
        public DateTime UpLoadDate { get; set; } = DateTime.Now;  

        public CourseModel Course { get; set; }
    }
}

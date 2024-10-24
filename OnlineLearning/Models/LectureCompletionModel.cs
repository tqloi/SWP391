using OnlineLearning.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    public class LectureCompletionModel
    {
        [Key]
        public int CompletionID { get; set; } // Primary key with Identity

        [Required]
        [StringLength(450)]

        public string UserID { get; set; } // Foreign key to AspNetUsers

        public int LectureID { get; set; } // Foreign key to Lecture

        public DateTime? CompletionDate { get; set; } // Nullable Completion Date

        // Navigation properties
        [ForeignKey("UserID")]
        public AppUserModel User { get; set; }

        [ForeignKey("LectureID")]
        public LectureModel Lecture { get; set; }
    }
}

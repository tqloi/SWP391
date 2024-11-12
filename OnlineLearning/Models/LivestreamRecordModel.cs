using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class LivestreamRecordModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LivestreamRecordID { get; set; }

        [Required]
        [StringLength(450)]
        public string UserID { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdateDate { get; set; }
        [StringLength(255)]
        public string Title { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ScheduleStartTime { get; set; } = new DateTime(2001, 1, 1, 0, 0, 0);

        [Column(TypeName = "time")]
        public TimeSpan? ScheduleLiveDuration { get; set; }

        [StringLength(255)]
        public string LivestreamId { get; set; }
        public int CourseID { get; set; }
       
        [ForeignKey("UserID")]
        public AppUserModel User { get; set; }

        [ForeignKey("CourseID")]
        public CourseModel Course { get; set; }
    }
}

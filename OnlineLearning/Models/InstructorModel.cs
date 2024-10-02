using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class InstructorModel
    {
        [Key]
        public string InstructorID { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        [ForeignKey("InstructorID")]
        public AppUserModel AppUser { get; set; }
    }
}

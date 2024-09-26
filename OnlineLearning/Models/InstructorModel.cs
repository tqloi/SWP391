using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class InstructorModel
    {
        [Key]
        public string InstructorId { get; set; }
        [Required]
        public string Decripstion { get; set; }
        public string Id { get; set; }
        public AppUserModel User { get; set; }

    }
}

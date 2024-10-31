using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class InstructorProfileViewModel
    {
        public AppUserModel User { get; set; }
        public InstructorModel Instructor { get; set; }
        public int TotalCourse { get; set; }
        public int TotalStudent { get; set; }
        public string Role { get; set; }
    }
}

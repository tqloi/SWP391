using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class LectureViewModel
    {

            [Required]
            public int CourseID { get; set; }
            [Required]
            public string Title { get; set; }
            [Required]
            public string Description { get; set; }
            public IFormFileCollection? LectureFile { get; set; }
            public IFormFile? VideoFile { get; set; }

    }
}

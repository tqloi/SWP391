using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineLearning.Models.ViewModel
{
    public class QuestionViewModel
    {
        [Key]
        public int QuestionID { get; set; }

        [ForeignKey("Test")]
        [Required]
        public int TestID { get; set; }

        [Required]
        [StringLength(255)]
        //the question itself 
        public string QuestionText { get; set; }

        [Required]
        [StringLength(255)]
        public string AnswerA { get; set; }

        [StringLength(255)]
        public string AnswerB { get; set; }

        [StringLength(255)]
        public string AnswerC { get; set; }

        [StringLength(255)]
        public string AnswerD { get; set; }

        [StringLength(255)]
        public required string CorrectAnswer { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; }

        public IFormFile? QuestionImage { get; set; }
        public required TestModel Test { get; set; }
    }
}

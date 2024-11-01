using OnlineLearning.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class ScoreModel
    {
        [Key]
        public int ScoreID { get; set; }

        [ForeignKey("AspNetUser")]
        public string StudentID { get; set; } // Corresponds to AspNetUsers(id)

        [ForeignKey("Test")]
        public int TestID { get; set; } // Corresponds to Test(testID)
        public DateTime DoTestAt { get; set; }

        [Required]
        public double Score { get; set; } // Score value
        public int NumberOfAttempt {  get; set; }

        // Navigation properties
        public required AppUserModel Student { get; set; } // Represents the user
        public required TestModel Test { get; set; } // Represents the test
    }
}

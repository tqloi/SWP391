using OnlineLearning.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ReviewModel
{
    [Key]
    public int ReviewID { get; set; }

    [ForeignKey("Course")]
    public int CourseID { get; set; }

    [ForeignKey("User")]
    public string UserID { get; set; }

    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
    public double Rating { get; set; }

    [MaxLength(255)]
    public string Comment { get; set; }

    public DateTime ReviewDate { get; set; }

    public CourseModel Course { get; set; }
    public AppUserModel User { get; set; }
}

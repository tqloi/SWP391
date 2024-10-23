using OnlineLearning.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ReportModel
{
    [Key]
    public int ReportID { get; set; }

    [Required]
    [StringLength(450)] 
    public string UserID { get; set; } 
    public string Subject { get; set; }
    [Required]
    public string Comment { get; set; } 

    [DataType(DataType.DateTime)]
    public DateTime FeedbackDate { get; set; } = DateTime.Now;

    [ForeignKey("UserID")]
    public AppUserModel User { get; set; } 
}

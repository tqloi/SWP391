namespace OnlineLearning.Models.ViewModel
{
    public class PaymentViewModel
    {
        public string UserID { get; set; }
        public string UserFullName { get; set; }
        public int CourseID { get; set; }
        public string CourseName { get; set; } 
        public decimal Price { get; set; } 
        public DateTime PaymentDate { get; set; } 
    }
}

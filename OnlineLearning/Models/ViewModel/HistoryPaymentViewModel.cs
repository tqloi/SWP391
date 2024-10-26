namespace OnlineLearning.Models.ViewModel
{
    public class HistoryPaymentViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; } 
        public double Amount { get; set; } 
        public DateTime Date { get; set; } 
        public string Description { get; set; }
        public string Status { get; set; }
    }
}

using OnlineLearning.Models;

namespace OnlineLearning.Areas.Admin.Models.ViewModel
{
    public class ListIndexViewModel
    {
        public List<AppUserModel> ListUser { get; set; }
        public List<PaymentModel> ListPayment { get; set; }
        public int ListCourses { get; set; }
    }
}

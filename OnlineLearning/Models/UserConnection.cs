namespace OnlineLearning.Models
{
    public class UserConnection
    {
        public string userID {  get; set; }
        public string CourseID { get; set; }
        public AppUserModel User { get; set; }
        public string HubRoom { get; set; }
    }
}

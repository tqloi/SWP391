namespace OnlineLearning.Models.ViewModel
{
    public class RoleViewModel
    {
        public IList<string> UserRoles { get; set; }
        public string UserID { get; set; }
        
        public List<AppUserModel> ListUser { get; set; }
    }
}

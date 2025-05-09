﻿namespace OnlineLearning.Models.ViewModel
{
    public class RoleViewModel
    {
        public IList<string> UserRoles { get; set; }
        public List<AppUserModel> ListUser { get; set; }
        public string ReceiverId { get; set; }
        public string SendId { get; set; }
        public string SendName { get; set; }
        public string ReceiveName { get; set; }
        public List<MessageModel> Messages { get; set; }
        public string IsReadmess { get; set; }
        public string receimess { get; set; }
        public string sendimg { get; set; }
        public string receiveimg { get; set; }
        public string Token { get; set; }
        public string sendcall { get; set; }
    }
}

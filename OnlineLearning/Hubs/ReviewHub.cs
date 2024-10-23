using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OnlineLearning.Controllers;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Hubs
{
    public class ReviewHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveMessage",$"{Context.ConnectionId} has joined"); 
        }

        

        public async Task SendReview()
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
        }
        
    }
}

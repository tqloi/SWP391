using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string senderId, string receiverId, string messageContent)
    {
        // Gửi tin nhắn đến người nhận
        await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, messageContent);
    }
}

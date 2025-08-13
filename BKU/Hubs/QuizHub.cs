using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace BKU.Hubs
{
   // [Authorize]
    public class QuizHub:Hub
    {
        public Task JoinRoom(string roomId) =>
           Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        public Task LeaveRoom(string roomId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }
}

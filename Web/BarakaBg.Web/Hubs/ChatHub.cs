using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace BarakaBg.Web.Hubs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class ChatHub : Hub
    {
        public ChatHub()
        {
        }

        public async Task Send(string message)
        {
            //await this.Clients
            //    .All
            //    .SendAsync("New Message",
            //        new Message
            //        {
            //            User = this.Context.User.Identity.Name,
            //            Text = message,
            //        });
        }
    }
}

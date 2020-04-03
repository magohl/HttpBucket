using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace HttpBucket.Hubs
{
    public class BucketHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var bucketId = httpContext.Request.Query["bucketId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, bucketId);


            await base.OnConnectedAsync();
        }
    }
}
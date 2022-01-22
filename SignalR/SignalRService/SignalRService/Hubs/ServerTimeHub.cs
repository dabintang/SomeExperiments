using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRService.Hubs
{
    /// <summary>
    /// 服务器时间信息
    /// </summary>
    public class ServerTimeHub : Hub<IServerTimeHub>
    {
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine($"连接成功：ConnectionId={Context.ConnectionId}");
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"断开连接：ConnectionId={Context.ConnectionId}");
        }

        /// <summary>
        /// 广播当前服务器时间
        /// </summary>
        /// <returns></returns>
        public async Task BroadcastTime()
        {
            await this.Clients.All.BroadcastTime(DateTime.Now);
        }
    }
}

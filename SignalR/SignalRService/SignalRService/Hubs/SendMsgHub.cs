using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRService.Hubs
{
    /// <summary>
    /// 发送消息
    /// </summary>
    public class SendMsgHub : Hub
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
        /// 发送给所有人
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendToAll(string msg)
        {
            await this.Clients.All.SendAsync("ReceiveMessage", msg);
        }

        /// <summary>
        /// 发送给自己
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendToSelf(string msg)
        {
            await this.Clients.Caller.SendAsync("ReceiveMessage", msg);
        }

        /// <summary>
        /// 发送给其他人
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendToOthers(string msg)
        {
            await this.Clients.Others.SendAsync("ReceiveMessage", msg);
        }
    }
}

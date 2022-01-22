using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tdb.framework.webapi.standard.Auth;

namespace SignalRService.Hubs
{
    /// <summary>
    /// 有身份信息
    /// </summary>
    [Authorize]
    public class AuthHub : Hub
    {
        /// <summary>
        /// key：uid
        /// value：ConnectionId
        /// </summary>
        private static Dictionary<string, string> dicUID_Conn = new Dictionary<string, string>();

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine($"连接成功：ConnectionId={Context.ConnectionId}");

            var uid = Context.User.FindFirst(TdbClaimTypes.UID).Value;
            var name = Context.User.Identity.Name;

            if (dicUID_Conn.ContainsKey(uid))
            {
                await this.Clients.Caller.SendAsync("ReceiveMessage", $"该uid已有链接，请勿重复链接");

                //如果该uid已经有连接了，则主动断开链接
                Context.Abort();
            }
            else
            {
                await this.Clients.Caller.SendAsync("ReceiveMessage", $"连接成功，uid={uid}，name={name}");
                dicUID_Conn[uid] = Context.ConnectionId;
            }
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

            if (dicUID_Conn.ContainsKey(Context.ConnectionId))
            {
                dicUID_Conn.Remove(Context.ConnectionId);
            }
        }

        /// <summary>
        /// 给指定uid发送消息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendToUID(string uid, string msg)
        {
            if (dicUID_Conn.ContainsKey(uid))
            {
                var connID = dicUID_Conn[uid];
                await this.Clients.Client(connID).SendAsync("ReceiveMessage", msg);
            }
        }
    }
}

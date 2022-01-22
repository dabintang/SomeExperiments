using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRService.Hubs
{
    /// <summary>
    /// 服务器时间
    /// </summary>
    public interface IServerTimeHub
    {
        /// <summary>
        /// 广播当前服务器时间
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        Task BroadcastTime(DateTime time);
    }
}

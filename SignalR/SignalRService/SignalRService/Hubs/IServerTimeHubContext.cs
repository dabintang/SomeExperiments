using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRService.Hubs
{
    /// <summary>
    /// 服务器时间
    /// </summary>
    public interface IServerTimeHubContext : IHubContext<ServerTimeHub>
    {

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRService.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    [Route("tdb/signalr/v{api-version:apiVersion}/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
    }
}

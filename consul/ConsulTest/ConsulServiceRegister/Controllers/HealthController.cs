using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsulServiceRegister.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// 健康检测
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Check")]
        public IActionResult Check()
        {
            return Ok();
        }
    }
}
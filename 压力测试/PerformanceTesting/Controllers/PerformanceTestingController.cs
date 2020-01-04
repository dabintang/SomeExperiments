using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PerformanceTesting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceTestingController : ControllerBase
    {
        /// <summary>
        /// CPU密集但数据量少的情况
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("MoreCPU_LessData")]
        public int MoreCPU_LessData()
        {
            int result = 0;
            for (var i = 0; i <= UInt16.MaxValue; i++)
            {
                result += i;
            }

            return result;
        }

        /// <summary>
        /// 睡眠
        /// </summary>
        /// <param name="time">睡眠时间（单位：毫秒）</param>
        /// <returns></returns>
        [HttpGet]
        [Route("SleepTime")]
        public IActionResult SleepTime(int time)
        {
            if (time > 0)
            {
                Thread.Sleep(time);
            }

            return new OkResult();
        }
    }
}
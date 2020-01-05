using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsulTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="name">服务名</param>
        /// <returns></returns>
        [HttpGet]
        [Route("FindService")]
        public List<string> FindService(string name)
        {
            using (var consul = new ConsulClient(c =>
            {
                c.Address = new Uri("http://120.79.229.45:8500/"); //Consul地址
            }))
            {
                //获取服务
                var services = consul.Catalog.Service(name).Result.Response;
                var list = services.Select(m => $"{m.Address}:{m.ServicePort}").ToList();

                return list;
            }
        }
    }
}
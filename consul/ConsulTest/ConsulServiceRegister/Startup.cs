using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsulServiceRegister
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //把本服务注册到Consul
            this.RegisterToConsul(config, appLifetime);
        }

        /// <summary>
        /// 把本服务注册到Consul
        /// </summary>
        /// <param name="config">参数配置</param>
        /// <param name="appLifetime">程序生命周期</param>
        private void RegisterToConsul(IConfiguration config, IHostApplicationLifetime appLifetime)
        {
            //Consul地址
            var consulClient = new ConsulClient(p => { p.Address = new Uri($"http://127.0.0.1:8500"); });

            //本地IP
            var localIP = this.GetLocalIP();
            //本地服务端口
            var localPort = Convert.ToInt32(config["port"]); //端口号从命令行参数获取（注：目前没找到直接获取本服务监听的端口的方法）

            //心跳检测设置
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(15), //服务停止多久后注销
                Interval = TimeSpan.FromSeconds(10), //间隔多久心跳检测一次
                HTTP = $"http://{localIP}:{localPort}/api/Health/Check", //心跳检查地址，本服务提供的地址
                Timeout = TimeSpan.FromSeconds(5)  //心跳检查超时时间
            };

            //服务名（这里通过命令行参数传入不同的服务名，模拟我们有不同的服务[其实只是同一个接口项目的不同运行实例]）
            var serviceName = config["service"];

            //注册信息
            var registration = new AgentServiceRegistration()
            {
                ID = $"{localIP}:{localPort}", //服务ID，唯一
                Name = serviceName, //服务名（如果服务搭集群，它们的服务名应该是一样的，但是ID不一样）
                Address = $"{localIP}", //服务地址
                Port = localPort, //服务端口
                Tags = new string[] { }, //服务标签，一般可以用来设置权重等本地服务特有信息
                Checks = new[] { httpCheck }, //心跳检测设置
            };

            //向Consul注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            //关闭程序后注销到Consul
            appLifetime.ApplicationStopped.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });
        }

        /// <summary>
        /// 获取本地IP
        /// </summary>
        /// <returns></returns>
        private string GetLocalIP()
        {
            var ip = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                    .Select(p => p.GetIPProperties())
                    .SelectMany(p => p.UnicastAddresses)
                    .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                    .FirstOrDefault()?.Address.ToString();
            return ip;
        }
    }
}

using Consul;
using System;

namespace ConsulServicesFound
{
    class Program
    {
        static void Main(string[] args)
        {
            //Consul配置
            var consulConfig = new ConsulClientConfiguration()
            {
                Address = new Uri("http://172.81.235.6:8500") //Consul地址
            };

            var consul = new ConsulClient((consulConfig) =>
            {
                consulConfig.Address = new Uri("http://172.81.235.6:8500"); //Consul地址
            });

            using (var consul = new ConsulClient(consulConfig))
            {
                var services = consul.Catalog.Service("MsgService").Result.Response;

                foreach (var s1 in services)
                {
                    Console.WriteLine($"ID={s1.ServiceID},Service={s1.ServiceName},Addr={s1.Address},Port={s1.ServicePort}");
                }

            }


            Console.WriteLine("Hello World!");
        }
    }
}

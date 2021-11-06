using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerAPIVer
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                //为true时，API会在响应的header中返回支持的版本信息
                o.ReportApiVersions = true;
                ////请求中未指定版本时默认为1.0
                //o.DefaultApiVersion = new ApiVersion(1, 0);
                ////版本号以什么形式，什么字段传递
                //o.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"));
                ////在不提供版本号时，默认为1.0  如果不添加此配置，不提供版本号时会报错"message": "An API version is required, but was not specified."
                //o.AssumeDefaultVersionWhenUnspecified = true;
                ////默认以当前最高版本进行访问
                //o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
            }).AddVersionedApiExplorer(o =>
            {
                //以通知swagger替换控制器路由中的版本并配置api版本
                o.SubstituteApiVersionInUrl = true;
                // 版本名的格式：v+版本号
                o.GroupNameFormat = "'v'VVV";
                ////未指定时采用默认版本
                //o.AssumeDefaultVersionWhenUnspecified = true;
            });

            //swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "API V1接口文档Title",
                    Description = "API V1接口文档Description"
                });

                c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v2",
                    Title = "API V2接口文档Title",
                    Description = "API V2接口文档Description"
                });

                //接口注释
                var xmlAPI = Path.Combine(AppContext.BaseDirectory, "SwaggerAPIVer.xml");
                c.IncludeXmlComments(xmlAPI, true);
            });

            services.AddControllers();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 Docs");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SignalRService.Hubs;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.framework.webapi.APIVersion;
using tdb.framework.webapi.Auth;
using tdb.framework.webapi.Cors;
using tdb.framework.webapi.standard.Exceptions;
using tdb.framework.webapi.standard.Log;
using tdb.framework.webapi.Swagger;

namespace SignalRService
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
            //日志
            services.AddTdbNLogger();
            //设置允许所有来源跨域
            services.AddTdbAllAllowCors();
            //添加身份认证与验权服务
            //services.AddTdbAuthJwtBearer("tangdabin20220108");
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = tdb.framework.webapi.standard.Auth.TdbClaimTypes.Name,
                    RoleClaimType = tdb.framework.webapi.standard.Auth.TdbClaimTypes.Role,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("tangdabin20220108")),
                    //不验Audience
                    ValidateAudience = false,
                    //不验Issuer
                    ValidateIssuer = false,
                    //允许的服务器时间偏移量
                    ClockSkew = TimeSpan.FromSeconds(10),
                };
                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        Logger.Ins.Fatal(context.Exception, "认证授权异常");
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 403;
                        context.Response.WriteAsync("权限不足");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.Clear();
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 401;
                        context.Response.WriteAsync("认证未通过");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = (context) =>
                    {
                        if (!context.HttpContext.Request.Path.HasValue)
                        {
                            return Task.CompletedTask;
                        }

                        //重点在于这里；判断是Signalr的路径
                        var accessToken = context.HttpContext.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!(string.IsNullOrWhiteSpace(accessToken)) && path.StartsWithSegments("/AuthHub"))
                        {
                            context.Token = accessToken;
                            return Task.CompletedTask;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            //授权
            services.AddAuthorization();
            //添加api版本控制及浏览服务
            services.AddTdbApiVersionExplorer();
            //swagger
            services.AddTdbSwaggerGenApiVer(o =>
            {
                o.ServiceCode = "tdb.signalR.demo";
                o.ServiceName = "SignalR Demo";
                o.LstXmlCommentsFileName.Add("SignalRService.xml");
            });

            //SignalR
            services.AddSignalR();

            //services.AddControllers();
            services.AddControllers(option =>
            {
                //异常处理
                option.AddTdbGlobalException();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //设置允许所有来源跨域
            app.UseTdbAllAllowCors();
            //认证
            app.UseAuthentication();
            //授权
            app.UseAuthorization();
            //swagger
            app.UseTdbSwaggerAndUIApiVer(provider);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ServerTimeHub>("/ServerTimeHub");
                endpoints.MapHub<SendMsgHub>("/SendMsgHub");
                endpoints.MapHub<AuthHub>("/AuthHub");
            });

            //广播服务器时间
            BroadcastTime(app);
        }

        //广播服务器时间
        private void BroadcastTime(IApplicationBuilder app)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var hubContext = app.ApplicationServices.GetService<IHubContext<ServerTimeHub, IServerTimeHub>>();
                    await hubContext.Clients.All.BroadcastTime(DateTime.Now);
                    await Task.Delay(1000);
                }
            });
        }
    }
}

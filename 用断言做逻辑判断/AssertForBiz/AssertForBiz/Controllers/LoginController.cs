using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssertForBiz.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AssertForBiz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public LoginRes Login(LoginReq req)
        {
            //断言
            Assert.IsTrue(() => req.LoginName == "user" && req.Pwd == "123", 1, "用户名或密码不对");

            //登录成功
            return new LoginRes() { IsOK = true };
        }
    }
}
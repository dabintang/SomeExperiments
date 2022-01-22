using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using tdb.framework.webapi.APIVersion;
using tdb.framework.webapi.standard.Auth;
using tdb.framework.webapi.standard.DTO;

namespace SignalRService.Controllers.V1
{
    /// <summary>
    /// 
    /// </summary>
    [TdbApiVersion(1)]
    public class AccountController : BaseController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns>token</returns>
        [HttpPost]
        [AllowAnonymous]
        public BaseItemRes<string> Login([FromBody] LoginReq req)
        {
            //用户信息
            var lstClaim = new List<Claim>();
            lstClaim.Add(new Claim(TdbClaimTypes.UID, req.UID));
            lstClaim.Add(new Claim(TdbClaimTypes.Name, req.Name));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(lstClaim),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("tangdabin20220108")), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return BaseItemRes<string>.Ok(tokenString);
        }

        ///// <summary>
        ///// 测试
        ///// </summary>
        ///// <returns>token</returns>
        //[HttpPost]
        //[Authorize]
        //public BaseItemRes<string> Test()
        //{
        //    return BaseItemRes<string>.Ok("1234");
        //}
    }

    /// <summary>
    /// 密码登录条件
    /// </summary>
    public class LoginReq
    {
        /// <summary>
        /// UID
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
    }
}

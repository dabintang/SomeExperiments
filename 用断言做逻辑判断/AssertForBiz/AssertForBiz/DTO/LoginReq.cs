using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssertForBiz.DTO
{
    /// <summary>
    /// 登陆请求
    /// </summary>
    public class LoginReq
    {
        /// <summary>
        /// 登陆名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
    }
}

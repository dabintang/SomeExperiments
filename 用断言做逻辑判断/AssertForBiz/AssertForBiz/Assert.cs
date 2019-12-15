using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssertForBiz
{
    /// <summary>
    /// 断言
    /// </summary>
    public class Assert
    {
        /// <summary>
        /// 断言为真
        /// </summary>
        /// <param name="errCode">错误码</param>
        /// <param name="errMsg">错误信息</param>
        /// <param name="func"></param>
        public static void IsTrue(Func<bool> func, int errCode, string errMsg)
        {
            if (func() == false)
            {
                ThrowException(errCode, errMsg);
            }
        }

        /// <summary>
        /// 抛异常
        /// </summary>
        /// <param name="errCode">错误码</param>
        /// <param name="errMsg">错误信息</param>
        private static void ThrowException(int errCode, string errMsg)
        {
            throw new AssertException() { ErrCode = errCode, ErrMsg = errMsg };
        }
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    public class AssertException : Exception
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
    }
}

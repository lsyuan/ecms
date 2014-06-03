using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ajax.Model
{
    /// <summary>
    /// 数据请求返回结果基础类
    /// </summary>
    public class BaseResult
    {
        /// <summary>
        /// 数据请求状态,0正常，1错误
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Errormsg { get; set; }
    }
}

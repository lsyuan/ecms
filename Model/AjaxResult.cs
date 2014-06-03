using System;

namespace Ajax.Model
{
    /// <summary>
    /// ajax操作返回结果
    /// </summary>
    public class AjaxResult
    {
        /// <summary>
        /// ajax请求处理状态
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// ajax请求返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 即将跳转的url
        /// </summary>
        public string Url { get; set; }
    }
}

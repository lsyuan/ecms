
using System;
namespace Ajax.DBUtility
{
    /// <summary>
    /// 表特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : BaseAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">表名</param>
        /// <param name="code">表编码</param>
        public TableAttribute(String name, String code)
        {
            base.Name = name;
            base.Code = code;
        }
    }
}

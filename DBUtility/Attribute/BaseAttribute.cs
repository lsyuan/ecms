using System;
using System.Collections.Generic;
using System.Text;

namespace Ajax.DBUtility
{
    /// <summary>
    /// 属性基类
    /// </summary>
    public class BaseAttribute : Attribute
    {
        /// <summary>
        /// 数据库对象名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库对象编码
        /// </summary>
        public string Code { get; set; }

        public BaseAttribute() { }

    }
}

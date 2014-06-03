using System;
using System.Collections.Generic;
using System.Text;

namespace Ajax.DBUtility
{
    /// <summary>
    /// 必填字段特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredAttribute : BaseAttribute
    {
    }
}

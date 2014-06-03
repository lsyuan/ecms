using System;
using System.Collections.Generic;
using System.Text;

namespace Ajax.DBUtility
{
    /// <summary>
    /// 验证特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidationAttribute : Attribute
    {
        private bool notNull;

        /// <summary>
        /// 非空验证
        /// </summary>
        public bool NotNull
        {
            get { return notNull; }
            set { notNull = value; }
        }

        public ValidationAttribute(bool notNull = false)
        {
            this.NotNull = notNull;
        }
    }
}

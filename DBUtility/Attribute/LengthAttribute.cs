using System;
using System.Collections.Generic;
using System.Text;

namespace Ajax.DBUtility
{
    /// <summary>
    /// 长度特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LengthAttribute : Attribute
    {
        private int maxSize;
        private int minSize;

        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }
        /// <summary>
        /// 最小长度
        /// </summary>
        public int MinSize
        {
            get { return minSize; }
            set { minSize = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="minSize">最小长度</param>
        /// <param name="maxSize">最大长度</param>
        public LengthAttribute(int minSize, int maxSize)
        {
            this.MinSize = minSize;
            this.MaxSize = maxSize;
        }
    }
}

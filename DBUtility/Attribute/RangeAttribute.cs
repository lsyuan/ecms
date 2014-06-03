using System;
using System.Collections.Generic;
using System.Text;

namespace Ajax.DBUtility
{
    /// <summary>
    /// 取值范围特性
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RangeAttribute : Attribute
    {
        private long minValue;
        private long maxValue;
        private int scale;

        /// <summary>
        /// 最小值
        /// </summary>
        public long MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public long MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
        /// <summary>
        /// 精度
        /// </summary>
        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="scale">精度  </param>
        public RangeAttribute(long minValue, long maxValue, int scale = 0)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Scale = scale;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ajax.Common
{
    /// <summary>
    /// easyUI tableGrid控件的分页参数
    /// </summary>
    public class EasyUIGridParamModel
    {
        string pageIndex = "1";
        string pageSize = "10";
        /// <summary>
        /// 当前页索引
        /// </summary>
        public string page
        {
            get { return string.IsNullOrEmpty(pageIndex) ? "1" : pageIndex; }
            set
            {
                if (!string.IsNullOrEmpty(value) && Convert.ToInt32(value) <= 0)
                {
                    value = "1";
                }
                pageIndex = value;
            }
        }
        /// <summary>
        /// 单页显示行数
        /// </summary>
        public string rows
        {
            get { return string.IsNullOrEmpty(pageSize) ? "10" : pageSize; }
            set
            {
                if (!string.IsNullOrEmpty(pageSize) && Convert.ToInt32(pageSize) <= 0)
                {
                    value = "10";
                }
                pageSize = value;
            }
        }
    }
}

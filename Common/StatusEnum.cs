using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ajax.Common
{
    /// <summary>
    /// 各状态字段的枚举
    /// </summary>
    public class StatusEnum
    {
        /// <summary>
        /// 协议状态
        /// </summary>
        public enum AgreeStatusEnum
        {
            /// <summary>
            /// 未审批
            /// </summary>
            NoApproval= 0,
            /// <summary>
            /// 已审批
            /// </summary>
            Approval=1,
            /// <summary>
            /// 已删除
            /// </summary>
            Delete=2
        }
    }
}

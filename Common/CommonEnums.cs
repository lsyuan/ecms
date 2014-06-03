using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ajax.Common
{
    public class CommonEnums
    {
        /// <summary>
        /// 客户状态枚举
        /// WaitCheck:待审核，Normal：正常，Deleted：已删除，PauseCharge：暂停缴费
        /// </summary>
        public enum CustomerState { WaitCheck = 0, Normal = 1, PauseCharge = 2, Deleted = 3, AuditUnPass = 4 }

        /// <summary>
        /// 系统参数枚举
        /// </summary>
        public struct SysParameterEnums
        {
            public static readonly string ArrearTips = "ArrearTips";
            public static readonly string CommissionRatio = "CommissionRatio";
            public static readonly string DelayChargeRatio = "DelayChargeRatio";
            public static readonly string CustomerAutoPass = "CustomerAutoPass";
            public static readonly string OtherChargeAutoPass = "OtherChargeAutoPass";
            public static readonly string TempChargeAutoPass = "TempChargeAutoPass";
			public static readonly string ContrastAutoPass = "ContrastAutoPass";
			public static readonly string ChargeAutoPass = "ChargeAutoPass";
        }
    }
}

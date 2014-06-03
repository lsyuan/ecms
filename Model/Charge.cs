using System;
using Ajax.DBUtility;
namespace Ajax.Model
{
    /// <summary>
    /// 缴费信息表 
    /// </summary>
    [Serializable]
    [Table("缴费信息表", "T_Charge")]
    [Key("PK_T_CHARGE", "ID")]
    public partial class Charge
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Charge()
        { }
        #region Model
        /// <summary>
        /// 
        /// </summary>
        [Column("ID", "ID", "varchar", 32)]
        public virtual string ID { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        [Column("客户", "CustomerID", "varchar", 32)]
        public string CustomerID { get; set; }
        /// <summary>
        /// 缴费总金额
        /// </summary>
        [Column("缴费总金额", "Money", "decimal", 12)]
        public decimal Money { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        [Column("缴费时间", "CreateDate", "DateTime")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 操作用户
        /// </summary>
        [Column("操作用户", "OperatorID", "varchar", 32)]
        public string OperatorID { get; set; }
        /// <summary>
        /// 是否协议缴费
        /// </summary>
        [Column("是否协议缴费", "IsAgreementCharge", "int")]
        public int IsAgreementCharge { get; set; }
        /// <summary>
        /// 缴费开始时间
        /// </summary>
        [Column("缴费开始时间", "BeginDate", "DateTime")]
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 缴费截止时间
        /// </summary>
        [Column("缴费截止时间", "EndDate", "DateTime")]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 缴费状态[0未审核，1已审核，2已退费，3无效]
        /// </summary>
        [Column("缴费状态", "Status", "int")]
        public int Status { get; set; }
        /// <summary>
        /// 协议ID，如果是协议缴费，记录对应协议的ID
        /// </summary>
		[Column("协议ID", "AgreementID", "varchar", 32)]
        public string AgreementID { get; set; }
        #endregion Model

    }
}


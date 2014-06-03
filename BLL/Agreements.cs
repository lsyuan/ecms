using System;
using System.Data;
using System.Collections.Generic;
using Ajax.Common;
using Ajax.Model;
using Ajax.DAL;
namespace Ajax.BLL
{
	/// <summary>
	/// 客户协议信息表
	/// </summary>
	public partial class AgreementsRule
	{
		private readonly Ajax.DAL.AgreementsDAL dal = new Ajax.DAL.AgreementsDAL();
		public AgreementsRule()
		{ }
		#region  Method
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(string ID)
		{
			return dal.Exists(ID);
		}
		/// <summary>
		/// 判断某用户是否有在用协议
		/// </summary>
		/// <param name="agreeeID"></param>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public bool Exists(string agreeeID, string customerID)
		{
			return dal.Exists(agreeeID, customerID);
		}
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(Ajax.Model.Agreements model)
		{
			if (!dal.Exists(model.ID, model.CustomerID))
			{
				dal.Add(model);
			}
			else
			{
				throw new Exception("协议编号已经存在或用户已有协议！");
			}
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Ajax.Model.Agreements model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(string ID)
		{

			return dal.Delete(ID);
		}
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string IDlist)
		{
			return dal.DeleteList(IDlist);
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Ajax.Model.Agreements GetModel(string ID)
		{
			return dal.GetModel(ID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Ajax.Model.Agreements GetModelByCache(string ID)
		{

			string CacheKey = "AgreementsModel-" + ID;
			object objModel = Ajax.Common.DataCache.GetCache(CacheKey);
			if (objModel == null)
			{
				try
				{
					objModel = dal.GetModel(ID);
					if (objModel != null)
					{
						int ModelCache = Ajax.Common.ConfigHelper.GetConfigInt("ModelCache");
						Ajax.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
					}
				}
				catch { }
			}
			return (Ajax.Model.Agreements)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Agreements> GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}

		/// <summary>
		/// 获取协议过期提醒
		/// <param name="top">显示行数</param>
		/// </summary>
		public List<dynamic> GetTipContracts(int top)
		{
			return dal.GetTipContracts(top);
		}


		/// <summary>
		/// 获取收费协议json
		/// </summary>
		/// <param name="param"></param>
		/// <param name="agree"></param>
		/// <param name="itemCount"></param>
		/// <returns></returns>
		public List<dynamic> SearchAgree(EasyUIGridParamModel param, Agreements agree, Ajax.Model.Customer c, out int itemCount)
		{
			return new Ajax.DAL.AgreementsDAL().SearchContrast(param, agree, c, out itemCount);
		}
		/// <summary>
		/// 拒绝协议
		/// </summary>
		/// <param name="guidList"></param>
		/// <param name="status"></param>
		public void RefuseAgree(List<string> guidList, StatusEnum.AgreeStatusEnum status)
		{
			new Ajax.DAL.AgreementsDAL().UpdateAgreeStatus(guidList, status);
		}
		/// <summary>
		/// 更改协议为已审批
		/// </summary>
		/// <param name="guidList"></param>
		/// <param name="status"></param>
		public void ApprovalAgree(List<string> guidList, StatusEnum.AgreeStatusEnum status)
		{
			new Ajax.DAL.AgreementsDAL().UpdateAgreeStatus(guidList, status);
		}
		/// <summary>
		/// 通过所有未审核的协议
		/// </summary>
		/// <param name="guids"></param>
		/// <returns></returns>
		public void ApprovalAll()
		{
			new AgreementsDAL().ApprovalAll();
		}

		/// <summary>
		/// 获取指定用户的协议
		/// </summary>
		/// <param name="customerID"></param>
		public object GetAgreementObjectByCustomerID(string customerID)
		{
			return new AgreementsDAL().GetAgreementObjectByCustomerID(customerID);
		}
		/// <summary>
		/// 获取指定用户的协议
		/// </summary>
		/// <param name="customerID"></param>
		public Agreements GetAgreementByCustomerID(string customerID)
		{
			return new AgreementsDAL().GetAgreementByCustomerID(customerID);
		}
         /// <summary>
        /// 获取剩余未缴纳的协议金额
        /// </summary>
        /// <param name="agreementID"></param>
        /// <returns></returns>
        public decimal GetLastAgreeFee(string agreementID)
        {
            return dal.GetLastAgreeFee(agreementID);
        }
		#endregion  Method
	}
}


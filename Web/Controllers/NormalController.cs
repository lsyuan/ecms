using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ajax.BLL;
using Web.Common;
using Ajax.Model;
using Ajax.Common;

namespace Web.Controllers
{
	/// <summary>
	/// 公告操作控制器
	/// </summary>
	public class NormalController : Controller
	{
		/// <summary>
		/// 公告查看页面
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.公告通知)]
		public ActionResult Message()
		{
			return View();
		}
		/// <summary>
		/// 新增公告页面
		/// </summary>
		/// <returns></returns> 
		[AccessFilter(PoupEnums.公告通知, AccessEnums.Add)]
		public ActionResult AddMessage()
		{
			//接收人列表绑定
			List<Operator> operatorList = new OperatorRule().GetModelList("");
			ViewBag.operatorList = operatorList;
			//AddMessageNew操作提示消息
			if (TempData["tipMsg"] == null)
			{
				TempData["tipMsg"] = "";
			}
			string msgID = Request.QueryString["msgID"];
			if (string.IsNullOrEmpty(msgID))
			{
				ViewBag.Msg = new OperatorMsg();
			}
			else
			{
				ViewBag.Msg = new OperatorMsgRule().GetModel(msgID);
			}
			return View();
		}
		/// <summary>
		/// 新增or编辑公告操作
		/// </summary>
		/// <param name="msg">消息实体</param>
		/// <param name="txtAcceptIDs">接受人的ID</param>
		/// <returns></returns>
		[ValidateInput(false)]
		[AccessFilter(PoupEnums.公告通知, AccessEnums.Add)]
		public JsonResult AddMessageNew(string AcceptIDS, string Title, string Content)
		{
			AjaxResult result = new AjaxResult();
			Message msg = new Message();
			msg.Content = Content;
			msg.Title = Title;
			try
			{
				MessageRule msgR = new MessageRule();
				if (string.IsNullOrEmpty(msg.ID))//新增
				{
					msg.ID = Guid.NewGuid().ToString("N");
					msg.OperatorID = MyTicket.CurrentTicket.UserID;
					msg.CreateDate = DateTime.Now;
					msgR.Add(msg);
					//不选接收人，默认发送给所有人
					if (string.IsNullOrEmpty(AcceptIDS))
					{
						List<Ajax.Model.Customer> acceptList = new CustomerRule().GetList("");
						foreach (Ajax.Model.Customer c in acceptList)
						{
							AcceptIDS += c.OperatorID + ",";
						}
					}
					string[] strAcceptIDs = AcceptIDS.Remove(AcceptIDS.Length - 1, 1).Split(',');//Request.Form["txtAcceptIDs"].Split(',');
					//接收人
					OperatorMsgRule omsgR = new OperatorMsgRule();
					List<OperatorMsg> oMsgList = new List<OperatorMsg>();
					foreach (string acceptID in strAcceptIDs)
					{
						if (string.IsNullOrEmpty(acceptID)) continue;
						OperatorMsg omsg = new OperatorMsg();
						omsg.ID = Guid.NewGuid().ToString("N");
						omsg.Status = 0;//默认为未读
						omsg.MsgID = msg.ID;
						omsg.OperatorID = acceptID;
						oMsgList.Add(omsg);
					}
					omsgR.AddMul(oMsgList);
					result.Success = true;
					result.Message = "公告已经成功发出。";
				}
				else//编辑
				{
					result.Success = msgR.Update(msg);
					result.Message = result.Success ? "公告已经成功修改。" : "公告修改失败！";
				}
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "操作失败：" + ex.Message;
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 返回json数据
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.公告通知)]
		public JsonResult Search(EasyUIGridParamModel gridParam, int status)
		{
			int itemCount = 0;
			MessageRule msgR = new MessageRule();
			List<dynamic> list = msgR.GetSearchJson(gridParam, status, MyTicket.CurrentTicket.UserID, out itemCount);
			var listResult = from m in list
							 select new
							 {
								 ID = m.ID,
								 TITLE = "<a >" + m.TITLE + "</a>",
								 CREATEDATE = m.CREATEDATE,
								 STATUS = m.STATUS,
								 VIEW = "<b class='btnView'>查看</b>"
							 };
			return Json(new { total = itemCount, rows = listResult }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取公告详细信息
		/// </summary>
		/// <param name="msgID">要读取的消息的ID</param>
		/// <returns></returns>  
		[AccessFilter(PoupEnums.公告通知)]
		public JsonResult GetMsgDetail(string msgID)
		{
			Message ms = new Message();
			if (!string.IsNullOrEmpty(msgID))
			{
				string operatorID = MyTicket.CurrentTicket.UserID;
				ms = new OperatorMsgRule().ReadMsg(msgID, operatorID); //消息已读 
			}
			else
			{
				throw new Exception("非法访问");
			}
			return Json(ms.Content, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 删除公告
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.公告通知, AccessEnums.Delete)]
		public JsonResult DeleteMsg()
		{
			string[] msgIDs = Request.Form["msgIDs"].Split(',');
			try
			{
				bool flag1 = new MessageRule().DeleteMul(new List<string>(msgIDs));
				//bool flag2 = new OperatorMsgRule().DeleteMul(new List<string>(msgIDs));
			}
			catch
			{
				return Json("公告删除失败！", JsonRequestBehavior.AllowGet);
			}
			return Json("公告删除成功.", JsonRequestBehavior.AllowGet);
		}

		#region ArrearTips
		[AccessFilter(PoupEnums.欠费提醒, AccessEnums.Read)]
		public ActionResult ArrearTips()
		{
			return View();
		}
		#endregion
	}
}

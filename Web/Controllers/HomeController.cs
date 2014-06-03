using System.Collections.Generic;
using System.Web.Mvc;
using Ajax.BLL;
using Web.Common;
using Ajax.Model;
using Ajax.Common;
using System;
using System.Linq;
using ECMS.Web.Providers.Types.Attributes;
using System.Web.Security;
using Web.Provider;
using Web.Providers.Types.Models;
using System.Web.Caching;
using System.Web;
namespace Web.Controllers
{
	public class HomeController : Controller
	{

		/// <summary>
		/// 系统主页
		/// </summary>
		/// <returns></returns>
		[AccessFilter(PoupEnums.主页)]
		public ActionResult Index()
		{
			if (MyTicket.CurrentTicket != null)
			{
				ViewData["userName"] = MyTicket.CurrentTicket.UserName;
				ViewData["groupName"] = MyTicket.CurrentTicket.GroupName;
			}
			else
			{
				return View("Login");
			}
			return View();
		}
		[AllowAnonymous]
		public ActionResult Login()
		{
			//UserManager.Logoff(Session, Response);
			return View();
		}
		/// <summary>
		/// 设置登录角色
		/// </summary>
		/// <returns></returns>
		public ActionResult SetLoginGroup(string groupID)
		{

			List<Ticket> currentTicketList = (List<Ticket>)HttpContext.Cache["UserList"] as List<Ticket>;
			foreach (Ticket t in currentTicketList)
			{
				if (t.GroupID == groupID)
				{
					HttpContext.Items["User"] = t;
					UserManager.ChangeRole(t, Response);
					//MyTicket.CurrentTicket = t;
					//记录登录日志
					LoginLog log = new LoginLog();
					log.OperatorID = t.UserID;
					log.CreateTime = DateTime.Now;
					log.Type = 1;
					log.ID = WebHelper.GetNewGuidUpper();
					new LoginLogRule().Add(log);
					break;
				}
			}
			return RedirectToAction("Index", "Home");
		}
		/// <summary>
		/// 用户登录
		/// </summary>
		/// <returns></returns>
		public ActionResult GoLogin(string userName, string pwd, string validateCode)
		{
			AjaxResult result = new AjaxResult();
			OperatorRule operatorRule = new OperatorRule();
#if DEBUG
			validateCode = Session["ValidateCode"].ToString();
#endif
			if (validateCode != Session["ValidateCode"].ToString())
			{
				result.Success = false;
				result.Message = "验证码输入错误。";
			}
			else
			{
				Logon logon = new Logon() { Password = pwd, Username = userName };
				if (UserManager.ValidateUser(logon, Response))
				{
					List<Ticket> currentTicketList = new List<Ticket>();
					if (HttpContext.Cache["UserList"] != null)
					{
						currentTicketList = HttpContext.Cache["UserList"] as List<Ticket>;
					}
					if (currentTicketList.Count == 1)
					{
						//MyTicket.CurrentTicket = currentTicketList[0]; //唯一角色的用户直接进入系统
						result.Success = true;
						result.Url = "/Home/Index";
						//记录登录日志
						LoginLog log = new LoginLog();
						log.OperatorID = MyTicket.CurrentTicket.UserID;
						log.CreateTime = DateTime.Now;
						log.Type = 1;
						log.ID = WebHelper.GetNewGuidUpper();
						new LoginLogRule().Add(log);
						return Json(result, JsonRequestBehavior.AllowGet);
					}
					else
					{
						return Json(currentTicketList, JsonRequestBehavior.AllowGet);
					}
				}
				else
				{
					result.Success = false;
					result.Message = "用户名或者密码错误。";
					return Json(result, JsonRequestBehavior.AllowGet);
				}
				List<dynamic> userList = operatorRule.Login(userName, pwd);
				if (userList == null || userList.Count == 0)
				{
					result.Success = false;
					result.Message = "用户名或者密码错误。";

				}
				else
				{
					List<Ticket> currentTicketList = new List<Ticket>();
					foreach (dynamic t in userList)
					{
						if (currentTicketList.Count<Ticket>(ct => ct.GroupName == t.GROUPNAME) > 0)
						{
							continue;//同一用户多账号相同角色去重复
						}
						Ticket myTicket = new Ticket();
						myTicket.DeptID = t.DEPTID;
						myTicket.DeptName = t.DEPTNAME;
						myTicket.EmployeeID = t.EMPID;
						myTicket.EmployeeName = t.EMPNAME;
						myTicket.GroupID = t.GROUPID;
						myTicket.GroupName = t.GROUPNAME;
						myTicket.UserID = t.ID;
						myTicket.UserName = t.OPERNAME;
						myTicket.IsAdmin = (t.ISADMIN == "1") ? true : false;
						//myTicket.VoteList = new GroupVoteRule().GetOperVotes(t.GROUPID, t.ID);//获取权限列表
						myTicket.VoteDic = new Dictionary<string, int>();
						foreach (OperatorVote item in new GroupVoteRule().GetOperVotes(t.GROUPID, t.ID))
						{
							myTicket.VoteDic.Add(item.PoupID, item.VoteType);
						}
						//myTicket.CurrentOperator = operatorRule.GetModel(t.ID);
						currentTicketList.Add(myTicket);
					}
					if (currentTicketList.Count == 1)
					{
						//MyTicket.CurrentTicket = currentTicketList[0];//唯一角色的用户直接进入系统
						result.Success = true;
						result.Url = "/Home/Index";
						//记录登录日志
						LoginLog log = new LoginLog();
						log.OperatorID = MyTicket.CurrentTicket.UserID;
						log.CreateTime = DateTime.Now;
						log.Type = 1;
						log.ID = WebHelper.GetNewGuidUpper();
						new LoginLogRule().Add(log);
					}
					else
					{
						Session["currentUserList"] = currentTicketList;//记录角色列表，等待用户选择
						return Json(currentTicketList, JsonRequestBehavior.AllowGet);
					}
				}
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#region 菜单


		/// <summary>
		/// 获取菜单的json数据
		/// </summary>
		/// <returns></returns>
		public ActionResult GetMenuJson()
		{
			if (MyTicket.CurrentTicket == null)
			{
				return RedirectToAction("Index");
			}
			if (SystemConst.MenuList.Count == 0)
			{

				SystemConst.MenuList = new PoupRule().GetMenuJson();
			}
			var menus = from menu in SystemConst.MenuList.OrderBy(m => m.Value)
						select new
						{
							menuId = menu.ID,
							text = menu.Name,
							url = menu.Path,
							pid = menu.PID
						};
			return Json(new { menus = menus }, JsonRequestBehavior.AllowGet);
		}
		/// <summary>
		/// 获取菜单数据（用于grid控件绑定，角色权限设置时使用）
		/// </summary>
		/// <returns></returns>
		public ActionResult GetMenuGridJson()
		{
			if (MyTicket.CurrentTicket == null)
			{
				return RedirectToAction("Index");
			}
			PoupRule poup = new PoupRule();
			List<Poup> poupList = poup.GetMenuJson();
			var showList = from poupInfo in poupList.Where(p => string.IsNullOrEmpty(p.PID) == false)
						   select new
						   {
							   ID = poupInfo.ID,
							   Name = string.Format("<labe id='{0}'>{1}</label>", poupInfo.ID, poupInfo.Name),
							   Vote1 = "<input type='checkbox' value='1' pID='{0}'/>",
							   Vote2 = "<input type='checkbox' value='2' pID='{0}'/>",
							   Vote3 = "<input type='checkbox' value='4' pID='{0}'/>",
							   Vote4 = "<input type='checkbox' value='8' pID='{0}'/>"
						   };
			return Json(new { total = showList.ToList().Count, rows = showList }, JsonRequestBehavior.AllowGet);
		}

		#endregion
		/// <summary>
		/// 获取验证码
		/// </summary>
		/// <returns></returns>
		public ActionResult GetValidateCode()
		{
			ValidateCode vCode = new ValidateCode();
			string code = vCode.CreateValidateCode(4);
			Session["ValidateCode"] = code;
#if DEBUG
			ViewData["ValidateCode"] = code;
#endif
			byte[] bytes = vCode.CreateValidateGraphic(code);
			return File(bytes, @"image/jpeg");
		}

		/// <summary>
		/// 无权限提示页面
		/// </summary>
		/// <returns></returns>
		public ActionResult NoVoteJson()
		{
			ViewData["Msg"] = "对不起，你不具有此权限！";
			return View();
		}
		/// <summary>
		/// 无权限提示页面
		/// </summary>
		/// <returns></returns>
		public ActionResult NoVote()
		{
			return View();
		}
		/// <summary>
		/// 退出登录
		/// </summary>
		/// <returns></returns>
		public ActionResult LoginOut()
		{
			//记录登录日志
			if (MyTicket.CurrentTicket != null)
			{
				LoginLog log = new LoginLog();
				log.OperatorID = MyTicket.CurrentTicket.UserID;
				log.CreateTime = DateTime.Now;
				log.Type = 0;
				log.ID = WebHelper.GetNewGuidUpper();
				new LoginLogRule().Add(log);
			}
			//MyTicket.CurrentTicket = null;
			Session["currentUserList"] = null;
			Session.Clear();
			UserManager.Logoff(Session, Response);
			return RedirectToAction("Login", "Home");
		}
	}
}

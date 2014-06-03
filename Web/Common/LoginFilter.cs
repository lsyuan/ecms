using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Web.Common
{
	/// <summary>
	/// action访问权限过滤器
	/// </summary>
	public class AccessFilter : FilterAttribute, IActionFilter
	{
		/// <summary>
		/// 请求路径
		/// </summary>
		public PoupEnums RequestResource { get; set; }

		/// <summary>
		/// 权限数组1(增),2(删),4(查),8(改)
		/// </summary>
		public AccessEnums VoteArray { get; set; }

		/// <summary>
		/// 是否是页面请求
		/// </summary>
		public bool IsPageRequest = false;

		/// <summary>
		/// action访问权限过滤器
		/// </summary>
		/// <param name="requestResource">页面地址</param>
		/// <param name="isJsonRequest">json数据请求true，view页面请求false</param>
		public AccessFilter(PoupEnums requestResource)
		{
			this.RequestResource = requestResource;
		}

		/// <summary>
		/// action访问权限过滤器
		/// </summary>
		/// <param name="requestResource">页面地址</param>
		/// <param name="isJsonRequest">json数据请求true，view页面请求false</param>
		/// <param name="voteArray">当前action需要的权限：1(增),2(删),4(查),8(改)</param>
		public AccessFilter(PoupEnums requestResource, AccessEnums voteArray)
		{
			this.RequestResource = requestResource;
			this.VoteArray = voteArray;
		}
		/// <summary>
		/// action访问权限过滤器
		/// </summary>
		/// <param name="requestResource">页面地址</param>
		/// <param name="isJsonRequest">json数据请求true，view页面请求false</param>
		/// <param name="voteArray">当前action需要的权限：1(增),2(删),4(查),8(改)</param>
		/// <param name="isPageRequest">是否是页面请求</param>
		public AccessFilter(PoupEnums requestResource, AccessEnums voteArray, bool isPageRequest)
		{
			this.RequestResource = requestResource;
			this.VoteArray = voteArray;
			this.IsPageRequest = isPageRequest;
		}

		/// <summary>
		/// action执行之前的处理
		/// </summary>
		/// <param name="filterContext"></param>
		void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
		{
			//登录验证
			if (MyTicket.CurrentTicket == null)
			{
				filterContext.HttpContext.Response.Redirect("~/Home/Login");
				return;
			}
			//权限过滤(超级管理员能够进行所有操作)
			if (!MyTicket.CurrentTicket.IsAdmin && (int)VoteArray != 0)
			{
				if (!HasAccess())
				{
					if (IsPageRequest)
					{
						filterContext.HttpContext.Response.Redirect(@"\Home\NoVote");
					}
					else
					{
						filterContext.HttpContext.Response.Redirect(@"\Home\NoVoteJson");
					}
				}
			}
		}
		/// <summary>
		/// action执行完成后的处理
		/// </summary>
		/// <param name="filterContext"></param>
		void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
		{
			//暂无处理
		}

		#region 自定义方法
		/// <summary>
		/// 访问权限判断
		/// </summary>
		/// <returns></returns>
		private bool HasAccess()
		{
			Ajax.Model.Poup pResult = SystemConst.MenuList.Find(delegate(Ajax.Model.Poup p)
			{
				return p.Value == RequestResource.ToString("D");
			});
			bool flag = false;
			if (pResult != null)
			{
				foreach (KeyValuePair<string, int> vote in MyTicket.CurrentTicket.VoteDic)
				{
					if (vote.Key == pResult.Path)
					{
						List<int> hasVotes = GetVoteArray(vote.Value);//拥有的权限
						int requireVote = (int)VoteArray;//要求的权限
						foreach (int v in hasVotes)
						{
							if (v == requireVote)
							{
								flag = true;
								break;
							}
						}
						break;
					}
				}
			}
			return flag;
		}
		/// <summary>
		/// 分解权限值为权限数组
		/// </summary>
		/// <param name="voteType">权限值</param>
		/// <returns></returns>
		private List<int> GetVoteArray(int voteType)
		{
			int[] returnArray;
			switch (voteType)
			{
				case 3:
					returnArray = new int[] { 1, 2 };
					break;
				case 5:
					returnArray = new int[] { 1, 4 };
					break;
				case 6:
					returnArray = new int[] { 2, 4 };
					break;
				case 9:
					returnArray = new int[] { 1, 8 };
					break;
				case 10:
					returnArray = new int[] { 2, 8 };
					break;
				case 12:
					returnArray = new int[] { 4, 8 };
					break;
				case 7:
					returnArray = new int[] { 1, 2, 4 };
					break;
				case 11:
					returnArray = new int[] { 1, 2, 8 };
					break;
				case 14:
					returnArray = new int[] { 2, 4, 8 };
					break;
				case 15:
					returnArray = new int[] { 1, 2, 4, 8 };
					break;
				default:
					returnArray = new int[] { voteType };
					break;
			}
			return new List<int>(returnArray);
		}
		#endregion
	}
}
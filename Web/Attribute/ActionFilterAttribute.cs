using System.Web.Mvc;
using System.Web;
using System;
using System.Collections.Generic;
using Web.Provider; 
namespace ECMS.Web.Provider.Attribute
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CCActionFilter : FilterAttribute, IActionFilter
	{
		public List<string> RightCode { get; set; }
		public CCActionFilter()
		{

		}
		/// <summary>
		///
		/// </summary>
		/// <param name="str">right code str , seprate by ',' if more than one righ code</param>
		public CCActionFilter(string str)
		{
			this.RightCode = new List<string>(str.Split(','));

		}
		public void OnActionExecuted(ActionExecutedContext filterContext)
		{

		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!HttpContext.Current.User.Identity.IsAuthenticated)
			{
				filterContext.HttpContext.Response.Redirect("/Home/Login");
			}
			else
			{
				// judge current user has request right or not , if not return no right json
				//if (RightCode != null)
				//{
				//	foreach (var item in RightCode)
				//	{
				//		if (!UserManager.User.Rights.Contains(item))
				//		{
				//			filterContext.HttpContext.Response.Redirect(@"\Home\NoRight");
				//			break;
				//		}
				//	}
				//}
			}
		}


	}
}
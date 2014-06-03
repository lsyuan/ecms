using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Web.Common;
using Web.Provider;
using Ajax.Common;

namespace Web
{
	// 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
	// 请访问 http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // 路由名称
				"{controller}/{action}/{id}", // 带有参数的 URL
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
			);

		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			#region 菜单初始化
			//Web.Common.SystemConst.MenuList = new Ajax.BLL.PoupRule().GetMenuJson();
			#endregion
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
			HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			if (authCookie != null)
			{
				// Get the forms authentication ticket.
				//string unZipCookie = ZipLib.UnZip(authCookie.Value);
				FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				var identity = new GenericIdentity(authTicket.Name, "Forms");
				var principal = new LoginPrincipal(identity);

				// Get the custom user data encrypted in the ticket.
				string userData = ((FormsIdentity)(Context.User.Identity)).Ticket.UserData;

				// Deserialize the json data and set it on the custom principal.
				var serializer = new JavaScriptSerializer();
				principal.User = (Ticket)serializer.Deserialize(userData, typeof(Ticket));

				// Set the context user.
				Context.User = principal;
			}
		}
	}
}
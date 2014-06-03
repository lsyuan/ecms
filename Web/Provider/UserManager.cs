using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Ajax.BLL;
using Web.Common;
using Web.Providers.Types.Models;
using Ajax.Common;
using Ajax.Model;
namespace Web.Provider
{
	public class UserManager
	{
		static UserManager()
		{
		}
		/// <summary>
		/// Returns the User from the Context.User.Identity by decrypting the forms auth ticket and returning the user object.
		/// </summary>
		public static Ticket User
		{
			get
			{
				if (HttpContext.Current.User.Identity.IsAuthenticated)
				{
					// The user is authenticated. Return the user from the forms auth ticket.
					return ((LoginPrincipal)(HttpContext.Current.User)).User;
				}
				else if (HttpContext.Current.Items.Contains("User"))
				{
					// The user is not authenticated, but has successfully logged in.
					return (Ticket)HttpContext.Current.Items["User"];
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Authenticates a user against a database, web service, etc.
		/// </summary>
		/// <param name="username">Username</param>
		/// <param name="password">Password</param>
		/// <returns>User</returns>
		public static List<Ticket> AuthenticateUser(string username, string password)
		{
			List<Ticket> currentTicketList = new List<Ticket>();
			OperatorRule operatorRule = new OperatorRule();
			List<dynamic> userList = operatorRule.Login(username, password);
			if (userList == null || userList.Count == 0)
			{
				return null;
			}
			else
			{
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
				//Cache["currentUserList"] = currentTicketList;
				return currentTicketList;
			}
		}

		/// <summary>
		/// Authenticates a user via the MembershipProvider and creates the associated forms authentication ticket.
		/// </summary>
		/// <param name="logon">Logon</param>
		/// <param name="response">HttpResponseBase</param>
		/// <returns>bool</returns>
		public static bool ValidateUser(Logon logon, HttpResponseBase response)
		{
			bool result = false;

			if (Membership.ValidateUser(logon.Username, logon.Password))
			{
				// Create the authentication ticket with custom user data.
				var serializer = new JavaScriptSerializer();
				string userData = serializer.Serialize(UserManager.User);

				FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
						logon.Username,
						DateTime.Now,
						DateTime.Now.AddDays(30),
						true,
						userData,
						FormsAuthentication.FormsCookiePath);

				// Encrypt the ticket.
				string encTicket = FormsAuthentication.Encrypt(ticket);

				//encTicket = ZipLib.Zip(encTicket);
				// Create the cookie.

				HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
				cookie.Expires = DateTime.Now.AddDays(1);
				cookie.Value = encTicket;
				response.AppendCookie(cookie);

				//response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

				result = true;
			}

			return result;
		}

		public static bool ChangeRole(Ticket currentTicket, HttpResponseBase response)
		{
			bool result = false;
			// Create the authentication ticket with custom user data.
			var serializer = new JavaScriptSerializer();
			string userData = serializer.Serialize(currentTicket);

			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
					currentTicket.UserName,
					DateTime.Now,
					DateTime.Now.AddDays(30),
					true,
					userData,
					FormsAuthentication.FormsCookiePath);
			// Encrypt the ticket.
			string encTicket = FormsAuthentication.Encrypt(ticket);
			// Create the cookie.
			response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

			return true;
		}

		/// <summary>
		/// Clears the user session, clears the forms auth ticket, expires the forms auth cookie.
		/// </summary>
		/// <param name="session">HttpSessionStateBase</param>
		/// <param name="response">HttpResponseBase</param>
		public static void Logoff(HttpSessionStateBase session, HttpResponseBase response)
		{
			// Delete the user details from cache.
			session.Abandon();
			// Delete the authentication ticket and sign out.
			FormsAuthentication.SignOut();
			// Clear authentication cookie.
			HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
			cookie.Expires = DateTime.Now.AddYears(-1);
			response.Cookies.Add(cookie);
		}
	}
}
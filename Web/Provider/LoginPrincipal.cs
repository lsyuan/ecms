using System.Security.Principal;
using Web.Common;

namespace Web.Provider
{
	public class LoginPrincipal : IPrincipal
	{
		public LoginPrincipal(IIdentity identity)
		{
			Identity = identity;
		}

		public IIdentity Identity
		{
			get;
			private set;
		}

		public Ticket User { get; set; }

		public bool IsInRole(string role)
		{
			return true;
		}
	}
}
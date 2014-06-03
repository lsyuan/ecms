using System.Collections;
using System.Web.Mvc;

namespace Web.Controllers
{
	public class BaseController : Controller
	{
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseController()
		{

		}
		private int pageIndex;

		/// <summary>
		/// 页索引
		/// </summary>
		public int PageIndex
		{
			get
			{
				if (Request.Params["start"] != null)
				{
					int.TryParse(Request.Params["start"].ToString(), out pageIndex);
				}
				return pageIndex;
			}
			set { pageIndex = value; }
		}
		private int pageSize;

		/// <summary>
		/// 页大小
		/// </summary>
		public int PageSize
		{
			get
			{
				if (Request.Params["limit"] != null)
				{
					int.TryParse(Request.Params["limit"].ToString(), out pageSize);
				}
				return pageSize;
			}
			set { pageSize = value; }
		}
		private ArrayList myParams;

		/// <summary>
		/// MVC请求的参数列表
		/// </summary>
		public ArrayList MyParams
		{
			get
			{
				if (Request.Params != null && Request.Params.Count > 0)
				{
					myParams = new ArrayList();
					for (int i = 0; i < Request.Params.Count; i++)
					{
						Param p = new Param();
						p.PramName = Request.Params.AllKeys[i];
						p.ParamValue = Request.Params[i];
						myParams.Add(p);
					}
				}
				return myParams;
			}
			set { myParams = value; }
		}

		/// <summary>
		/// 参数的结构体
		/// </summary>
		struct Param
		{
			public string PramName;
			public string ParamValue;
		}
		/// <summary>
		/// 00000000000000000000000000000000
		/// </summary>
		public const string GUIDEMPTY = "00000000000000000000000000000000";
	}
}
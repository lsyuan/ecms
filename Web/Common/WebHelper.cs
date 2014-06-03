using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Common
{
	public class WebHelper
	{
		/// <summary>
		/// 生成大写的GUID字符串	
		/// </summary>
		/// <returns></returns>
		public static string GetNewGuidUpper()
		{
			return Guid.NewGuid().ToString("N").ToUpper();
		}
	}
}
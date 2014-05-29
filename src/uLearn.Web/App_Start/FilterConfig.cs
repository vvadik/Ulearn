using System.Web.Mvc;

namespace uLearn.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(new RequireHttpsAttribute());
		}
	}
}
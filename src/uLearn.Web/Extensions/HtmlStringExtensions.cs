using System.Web;

namespace uLearn.Web.Extensions
{
	public static class HtmlStringExtensions
	{
		public static HtmlString ToLegacyHtmlString(this global::Microsoft.AspNetCore.Html.HtmlString aspNetCoreHtmlString)
		{
			return new HtmlString(aspNetCoreHtmlString.Value);
		}
	}
}
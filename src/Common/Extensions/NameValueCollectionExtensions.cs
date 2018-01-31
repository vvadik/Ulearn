using System.Collections.Specialized;
using System.Web;

namespace Ulearn.Common.Extensions
{
	public static class NameValueCollectionExtensions
	{
		public static string ToQueryString(this NameValueCollection collection)
		{
			var qs = HttpUtility.ParseQueryString("");
			qs.Add(collection);
			return qs.ToString();
		}
	}
}

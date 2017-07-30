using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace uLearn.Extensions
{
	public static class UriExtensions
	{
		public static string AddQueryParameter(this string url, string name, string value)
		{
			var uriBuilder = new UriBuilder(url);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query[name] = value;
			uriBuilder.Query = query.ToString();
			return uriBuilder.ToString();
		}

		public static string RemoveQueryParameter(this string url, string name)
		{
			var uriBuilder = new UriBuilder(url);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query.Remove(name);
			uriBuilder.Query = query.ToString();
			return uriBuilder.ToString();
		}
	}
}
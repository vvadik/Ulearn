using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Ulearn.Common.Extensions;

namespace Ulearn.Common
{
	public class HttpValueCollection : NameValueCollection
	{
		public HttpValueCollection(string queryString)
		{
			var dict = QueryHelpers.ParseQuery(queryString);
			foreach (var d in dict)
				d.Value.ForEach(s => Add(d.Key, s));
		}
		public override string ToString()
		{
			var pairs = this.Cast<string>().Select(key => (key, this[key])).ToDictionary(t => t.Item1, t => t.Item2);
			return QueryHelpers.AddQueryString("", pairs);
		}
	}
	
	public class WebUtils
	{
		public static HttpValueCollection ParseQueryString(string query)
		{
			return new HttpValueCollection(query);
		}
	}
}
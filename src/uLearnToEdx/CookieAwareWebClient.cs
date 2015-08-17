using System;
using System.Net;

namespace uLearn.CourseTool
{
	public class CookieAwareWebClient : WebClient
	{
		public CookieContainer CookieContainer { get; set; }
		public string CsrfToken { get; set; }

		public CookieAwareWebClient()
		{
			CookieContainer = new CookieContainer();
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			var request = base.GetWebRequest(address);
			var httpRequest = request as HttpWebRequest;
			if (httpRequest != null)
			{
				httpRequest.CookieContainer = CookieContainer;
				httpRequest.Headers["X-CSRFToken"] = CsrfToken;
				return httpRequest;
			}
			return request;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			var response = base.GetWebResponse(request);
			var setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];
			if (setCookieHeader != null)
			{
				CookieContainer.SetCookies(request.RequestUri, setCookieHeader);
				var csrfToken = CookieContainer.GetCookies(request.RequestUri)["csrftoken"];
				if (csrfToken != null)
					CsrfToken = csrfToken.Value;
			}
			return response;
		}
	}
}

using uLearn.Configuration;

/* Move it to Web.Api.Configuration after disabling Ulearn.Web. Now it's here because Ulearn.Web should know about CookieKeyRingDirectory */

namespace Web.Api.Configuration
{
	public class WebApiConfiguration : UlearnConfiguration
	{
		public UlearnWebConfiguration Web { get; set; }
	}

	public class UlearnWebConfiguration
	{
		public string CookieKeyRingDirectory { get; set; }
		public string CookieName { get; set; }
		public string CookieDomain { get; set; }
		public bool CookieSecure { get; set; }
	}
}
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using uLearn.Web.Microsoft.Owin.Security.VK;

namespace uLearn.Web
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Enable the application to use a cookie to store information for the signed in user
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login")
			});
			// Use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			// Uncomment the following lines to enable logging in with third party login providers
			//app.UseMicrosoftAccountAuthentication(
			//    clientId: "",
			//    clientSecret: "");


//			app.UseTwitterAuthentication(
//				consumerKey: "hC6XpJy0OPVkbvGzRIOJRA",
//				consumerSecret: "cEDewTtU7RKHimj2D1IpD75HUKnjVeobdSNhjAAQ");

			app.UseVkAuthentication(
				appId: "4381546",
				appSecret: "rqrWfJMYUT6Y3io91B3B");

			//app.UseFacebookAuthentication(
			//   appId: "",
			//   appSecret: "");

//			app.UseGoogleAuthentication();
		}
	}
}
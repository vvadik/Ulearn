using Microsoft.Owin;
using Owin;
using uLearn.Web;

[assembly: OwinStartup(typeof (Startup))]

namespace uLearn.Web
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
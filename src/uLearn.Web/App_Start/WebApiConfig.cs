using System.Web.Http;
using Microsoft.Owin.Security.OAuth;

namespace uLearn.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			config.SuppressDefaultHostAuthentication();
			config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings = JsonConfig.GetSettings();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

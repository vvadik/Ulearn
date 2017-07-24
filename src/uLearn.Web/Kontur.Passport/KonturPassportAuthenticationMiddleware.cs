using log4net;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using SkbKontur.Passport.Client;
using SkbKontur.Passport.Client.Configuration;

namespace uLearn.Web.Kontur.Passport
{
	public class KonturPassportAuthenticationMiddleware : AuthenticationMiddleware<KonturPassportAuthenticationOptions>
	{
		private readonly PassportClient passportClient;
		private readonly ILog log;

		public KonturPassportAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, KonturPassportAuthenticationOptions options)
			: base(next, options)
		{
			log = LogManager.GetLogger(typeof(KonturPassportAuthenticationMiddleware));
			var clientConfiguration = new PassportClientConfiguration
			{
				ClientId = Options.ClientId,
				ClientSecret = Options.ClientSecret,
				Scope = "profile email",
			};
			var clientOptions = new ClientOptions(clientConfiguration);
			passportClient = new PassportClient(clientOptions);

			if (Options.StateDataFormat == null)
			{
				var dataProtector = app.CreateDataProtector(
					typeof(KonturPassportAuthenticationMiddleware).FullName,
					Options.AuthenticationType,
					"v1");
				Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
			}
		}

		protected override AuthenticationHandler<KonturPassportAuthenticationOptions> CreateHandler()
		{
			return new KonturPassportAuthenticationHandler(passportClient, log);
		}
	}
}
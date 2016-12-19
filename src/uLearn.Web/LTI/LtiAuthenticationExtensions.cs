using System;
using System.Collections.Specialized;
using System.Data.Entity;
using log4net;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.OAuth;
using LtiLibrary.Owin.Security.Lti;
using LtiLibrary.Owin.Security.Lti.Provider;
using Microsoft.AspNet.Identity;
using Owin;
using uLearn.Web.DataContexts;
using uLearn.Web.Extensions;

namespace uLearn.Web.LTI
{
	public static class LtiAuthenticationExtensions
	{
		private static ILog log = LogManager.GetLogger(typeof(LtiAuthenticationExtensions));

		private static string LogSignatureGeneration(string httpMethod, Uri url, NameValueCollection parameters, string consumerSecret)
		{
			log.Info($"HTTP method: {httpMethod}");
			log.Info($"Url: {url}");
			log.Info($"Parameters: {parameters}");
			log.Info($"Consumer secret: {consumerSecret}");

			UrlEncodingParser urlEncodingParser = new UrlEncodingParser(Uri.UnescapeDataString(url.Query));
			parameters.Add((NameValueCollection)urlEncodingParser);
			log.Info("Run OAuthUtility.GenerateSignature(httpMethod, url, parameters, consumerSecret)");
			string signature = OAuthUtility.GenerateSignature(httpMethod, url, parameters, consumerSecret);
			foreach (string allKey in urlEncodingParser.AllKeys)
				parameters.Remove(allKey);
			log.Info($"signature: {signature}");
			return signature;
		}

		public static IAppBuilder UseLtiAuthentication(this IAppBuilder app)
		{
			app.UseLtiAuthentication(new LtiAuthenticationOptions
			{
				Provider = new LtiAuthenticationProvider
				{
					// Look up the secret for the consumer
					OnAuthenticate = async context =>
					{
						// Make sure the request is not being replayed
						var timeout = TimeSpan.FromMinutes(5);
						var oauthTimestampAbsolute = OAuthConstants.Epoch.AddSeconds(context.LtiRequest.Timestamp);
						if (DateTime.UtcNow - oauthTimestampAbsolute > timeout)
						{
							throw new LtiException("Expired " + OAuthConstants.TimestampParameter);
						}

						var db = new ULearnDb();
						var consumer = await db.Consumers.SingleOrDefaultAsync(c => c.Key == context.LtiRequest.ConsumerKey);
						if (consumer == null)
						{
							throw new LtiException("Invalid " + OAuthConstants.ConsumerKeyParameter + " " + context.LtiRequest.ConsumerKey);
						}

						/* Substitute http(s) scheme with real scheme from header */
						var uriBuilder = new UriBuilder(context.LtiRequest.Url)
						{
							Scheme = context.OwinContext.Request.GetRealRequestScheme(),
                            Port = context.OwinContext.Request.GetRealRequestPort()
						};
						context.LtiRequest.Url = uriBuilder.Uri;

						LogSignatureGeneration(context.LtiRequest.HttpMethod, context.LtiRequest.Url,
							new NameValueCollection(context.LtiRequest.Parameters), consumer.Secret);

						var signature = context.LtiRequest.GenerateSignature(consumer.Secret);

						log.Info($"Signature from request: {context.LtiRequest.Signature}");

						if (!signature.Equals(context.LtiRequest.Signature))
						{
							throw new LtiException("Invalid " + OAuthConstants.SignatureParameter);
						}

						// If we made it this far the request is valid
					},

					// Sign in using application authentication. This handler will create a new application
					// user if no matching application user is found.
					OnAuthenticated = context => SecurityHandler.OnAuthenticated(context),

					// Generate a username using the LisPersonEmailPrimary from the LTI request
					OnGenerateUserName = context => SecurityHandler.OnGenerateUserName(context)
				},
				SignInAsAuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
			});

			return app;
		}
	}
}

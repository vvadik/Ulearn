using System;
using System.Web.Configuration;
using Serilog;
using Ulearn.Core.Configuration;
using Ulearn.VideoAnnotations.Api.Client;

namespace uLearn.Web.Clients
{
	public static class UlearnVideoAnnotationsClient
	{
		private static readonly VideoAnnotationsClientConfiguration configuration
			= ApplicationConfiguration.Read<UlearnConfiguration>().VideoAnnotationsClient;

		public static readonly VideoAnnotationsClient Instance =
			string.IsNullOrEmpty(configuration?.Endpoint)
				? null
				: new VideoAnnotationsClient(
					new LoggerConfiguration().WriteTo.Log4Net().CreateLogger(),
					configuration.Endpoint
				);
	}
}
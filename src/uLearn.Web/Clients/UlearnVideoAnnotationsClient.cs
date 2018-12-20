using System;
using System.Web.Configuration;
using Serilog;
using Ulearn.VideoAnnotations.Api.Client;

namespace uLearn.Web.Clients
{
	public static class UlearnVideoAnnotationsClient
	{
		public static VideoAnnotationsClient Instance = new VideoAnnotationsClient(
			new LoggerConfiguration().WriteTo.Log4Net().CreateLogger(),
			new Uri(WebConfigurationManager.AppSettings["ulearn.videoAnnotations.endpoint"])
		);
	}
}
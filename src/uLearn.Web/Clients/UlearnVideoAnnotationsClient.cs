using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using RestSharp.Extensions;
using Serilog;
using Serilog.Core;
using Ulearn.Common.Api;
using Ulearn.Common.Extensions;
using Ulearn.VideoAnnotations.Api.Client;
using Ulearn.VideoAnnotations.Api.Models.Responses.Annotations;

namespace uLearn.Web.Clients
{
	public static class UlearnVideoAnnotationsClient
	{
		public static readonly VideoAnnotationsClient Instance = new VideoAnnotationsClient(
			new LoggerConfiguration().WriteTo.Log4Net().CreateLogger(),
			new Uri(WebConfigurationManager.AppSettings["ulearn.videoAnnotations.endpoint"])
		);
	}
}
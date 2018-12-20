using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using RestSharp.Extensions;
using Serilog;
using Serilog.Core;
using Ulearn.Common.Api;
using Ulearn.Common.Extensions;
using Ulearn.VideoAnnotations.Api.Models.Responses.Annotations;

namespace uLearn.Web.Clients
{
	public class UlearnVideoAnnotationsClient
	{
		private readonly Logger logger;
		private readonly Uri url;
		private readonly HttpClient httpClient;

		public static readonly UlearnVideoAnnotationsClient Instance = new UlearnVideoAnnotationsClient(
			new LoggerConfiguration().WriteTo.Log4Net().CreateLogger(),
			new Uri(WebConfigurationManager.AppSettings["ulearn.videoAnnotations.endpoint"])
		);
		
		private UlearnVideoAnnotationsClient(Logger logger, Uri url)
		{
			this.logger = logger;
			this.url = url;

			httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10), BaseAddress = url };
		}

		/* This method is almost copy-paste from BaseApiClient. But we can't use BaseApiClient (or VideoAnnotationsClient) here because in this case
		   we should reference Microsoft.AspNetCore.Mvc.Core, but it's inappropriate in Ulearn.Web.
		   In the future (where there is no Ulearn.Web and only Web.Api exists) we should remove this class and use only VideoAnnotationsClient */
		public async Task<AnnotationsResponse> GetAnnotationsAsync(string googleDocId, string videoId)
		{
			HttpResponseMessage response;
			try
			{
				response = await httpClient.GetAsync(httpClient.BaseAddress + "annotations?google_doc_id=" + googleDocId.UrlEncode() + "&video_id=" + videoId.UrlEncode()).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				logger.Error(e, "Can't send request to video annotations service: {message}", e.Message);
				throw new ApiClientException($"Can't send request to video annotations service: {e.Message}", e);
			}
			
			if (!response.IsSuccessStatusCode)
			{
				logger.Error("Bad response code from video annotations service: {statusCode} {statusCodeDescrption}", (int) response.StatusCode, response.StatusCode.ToString());
				throw new ApiClientException($"Bad response code from video annotations service: {(int)response.StatusCode} {response.StatusCode}");
			}

			AnnotationsResponse result;
			try
			{
				result = await response.Content.ReadAsJsonAsync<AnnotationsResponse>().ConfigureAwait(false);
			}
			catch (Exception e)
			{
				logger.Error(e, "Can't parse response from video annotations service: {message}", e.Message);
				throw new ApiClientException($"Can't parse response from video annotations service: {e.Message}", e);
			}

			return result;
		}
	}
}
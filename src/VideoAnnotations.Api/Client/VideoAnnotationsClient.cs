using System;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using Ulearn.Common.Api;
using Ulearn.Common.Api.Models.Parameters;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.VideoAnnotations.Api.Models.Parameters.Annotations;
using Ulearn.VideoAnnotations.Api.Models.Responses.Annotations;

namespace Ulearn.VideoAnnotations.Api.Client
{
	public class VideoAnnotationsClient : BaseApiClient, IVideoAnnotationsClient
	{
		public VideoAnnotationsClient(ILogger logger, Uri endpointUrl)
			:base(logger, new ApiClientSettings
			{
				EndpointUrl = endpointUrl,
				ServiceName = "video annotations service",
			})
		{
		}

		public Task<AnnotationsResponse> GetAnnotationsAsync(AnnotationsParameters parameters)
		{
			return MakeRequestAsync<AnnotationsParameters, AnnotationsResponse>(HttpMethod.Get, Urls.Annotations, parameters);
		}
		
		public Task ClearAsync()
		{
			return MakeRequestAsync<ApiParameters, ApiResponse>(HttpMethod.Post, Urls.Clear, null);
		}
	}
}
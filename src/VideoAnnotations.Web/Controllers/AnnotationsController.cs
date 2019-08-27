using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.VideoAnnotations.Api.Models.Parameters.Annotations;
using Ulearn.VideoAnnotations.Api.Models.Responses.Annotations;
using Ulearn.VideoAnnotations.Web.Annotations;

namespace Ulearn.VideoAnnotations.Web.Controllers
{
	[Route("/")]
	public class AnnotationsController : BaseController
	{
		private readonly IGoogleDocApiClient googleDocApiClient;
		private readonly IAnnotationsParser annotationsParser;
		private readonly IAnnotationsCache annotationsCache;

		public AnnotationsController(IGoogleDocApiClient googleDocApiClient, IAnnotationsParser annotationsParser, IAnnotationsCache annotationsCache)
		{
			this.googleDocApiClient = googleDocApiClient;
			this.annotationsParser = annotationsParser;
			this.annotationsCache = annotationsCache;
		}

		[HttpGet(Api.Urls.Annotations)]
		public async Task<ActionResult<AnnotationsResponse>> Annotations([FromQuery] AnnotationsParameters parameters)
		{
			Dictionary<string, Annotation> annotations;
			if (!annotationsCache.TryGet(parameters.GoogleDocId, out annotations))
			{
				lock (annotationsCache)
				{
					if (!annotationsCache.TryGet(parameters.GoogleDocId, out annotations))
					{
						var googleDocContent = googleDocApiClient.GetGoogleDocContentAsync(parameters.GoogleDocId).Result;
						annotations = annotationsParser.ParseAnnotations(googleDocContent);

						annotationsCache.Add(parameters.GoogleDocId, annotations);
					}
				}
			}

			if (!annotations.TryGetValue(parameters.VideoId, out var annotation))
				return NotFound(new ErrorResponse($"Annotation for video {parameters.VideoId} not found in document {parameters.GoogleDocId}"));
			
			return new AnnotationsResponse
			{
				Annotation = annotation,
			};
		}

		[HttpPost(Api.Urls.Clear)]
		public IActionResult Clear()
		{
			annotationsCache.Clear();
			return Ok(new SuccessResponseWithMessage("Cache successfully cleared"));
		}
	}
}
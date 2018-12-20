using System.Collections.Generic;
using Ulearn.Common;
using Ulearn.VideoAnnotations.Api.Models.Responses.Annotations;

namespace Ulearn.VideoAnnotations.Web.Annotations
{
	public class AnnotationsCache : LruCache<string, Dictionary<string, Annotation>>, IAnnotationsCache
	{
		public AnnotationsCache(int capacity=50)
			: base(capacity)
		{
		}
	}
}
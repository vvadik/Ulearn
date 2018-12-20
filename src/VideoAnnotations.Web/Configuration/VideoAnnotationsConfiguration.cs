using System;
using Ulearn.Core.Configuration;

namespace Ulearn.VideoAnnotations.Web.Configuration
{
	public class VideoAnnotationsConfiguration : UlearnConfiguration
	{
		public VideoAnnotationsServiceConfiguration VideoAnnotations { get; set; }
	}

	public class VideoAnnotationsServiceConfiguration
	{
		public string GoogleDocsApiKey { get; set; }
		
		public TimeSpan Timeout { get; set; }
	}
}
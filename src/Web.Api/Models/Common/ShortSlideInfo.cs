using System;
using System.Runtime.Serialization;
using Ulearn.Core.Courses.Slides;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class ShortSlideInfo
	{
		[DataMember(Name = "id")]
		public Guid Id { get; set; }
		
		[DataMember(Name = "title")]
		public string Title { get; set; }

		/// <summary>
		/// Человекочитаемый фрагмент url для слайда
		/// </summary>
		[DataMember(Name = "slug")]
		public string Slug { get; set; }

		[DataMember(Name = "max_score")]
		public int MaxScore { get; set; }
		
		[DataMember(Name = "type")]
		public SlideType Type { get; set; }

		[DataMember(Name = "api_url")]
		public string ApiUrl { get; set; }
	}
}
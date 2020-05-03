using System.Runtime.Serialization;
using Ulearn.Core.Courses.Slides.Blocks.Api;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class ApiSlideInfo : ShortSlideInfo
	{
		[DataMember]
		public IApiSlideBlock[] Blocks { get; set; }
	}
}
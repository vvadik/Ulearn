using System.ComponentModel;
using System.Runtime.Serialization;
using Ulearn.Core;
using Ulearn.Core.Courses.Slides.Blocks;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	public class CodeBlockResponse : IApiSlideBlock
	{
		[DefaultValue(false)]
		[DataMember(Name = "hide", EmitDefaultValue = false)]
		public bool Hide { get; set; }
		
		[DataMember(Name = "code")]
		public string Code { get; set; }

		[DataMember(Name = "language")]
		public Language? Language { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; } = "code";

		public CodeBlockResponse(CodeBlock codeBlock)
		{
			Hide = codeBlock.Hide;
			Code = codeBlock.Code;
			Language = codeBlock.Language;
		}

		public CodeBlockResponse()
		{
		}
	}
}
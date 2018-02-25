using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AntiPlagiarism.Api.Models
{
	[JsonConverter(typeof(StringEnumConverter), true)]
	public enum Language : short
	{
		CSharp = 1,
	}
}
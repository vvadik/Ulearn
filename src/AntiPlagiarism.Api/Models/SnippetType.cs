using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AntiPlagiarism.Api.Models
{
	[JsonConverter(typeof(StringEnumConverter), true)]
	public enum SnippetType : short
	{
		TokensKindsOnly = 1,
		TokensKindsAndValues = 2,
	}
}
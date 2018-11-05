using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ulearn.Web.Api.Models.Responses
{
	public class ApiResponse
	{
		
	}

	[JsonConverter(typeof(StringEnumConverter), true)]
	public enum ResponseStatus
	{
		Ok,
		Error,
	}
}
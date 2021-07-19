using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses
{
	[DataContract]
	public class GoogleSheetsTaskListResponse
	{
		[DataMember]
		public List<GoogleSheetsTaskResponse> GoogleSheetsTasks { get; set; }
	}
}
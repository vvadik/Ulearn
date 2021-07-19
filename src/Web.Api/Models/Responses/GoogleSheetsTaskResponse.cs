using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses
{
	[DataContract]
	public class GoogleSheetsTaskResponse //TODO спросить Славу про наследование от SuccessResponse
	{
		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public ShortUserInfo AuthorInfo { get; set; }

		[DataMember]
		public List<int> GroupsIds { get; set; }

		[DataMember]
		public bool IsVisibleForStudents { get; set; }

		[DataMember]
		public DateTime? RefreshStartDate { get; set; }
		
		[DataMember]
		public DateTime? RefreshEndDate { get; set; }
		
		[DataMember]
		public int? RefreshTimeInMinutes { get; set; }
		
		[DataMember]
		public string SpreadsheetId { get; set; }
		
		[DataMember]
		public int ListId { get; set; }
	}
}
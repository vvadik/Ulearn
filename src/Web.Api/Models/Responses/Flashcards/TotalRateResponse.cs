using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Flashcards
{
	[DataContract]
	public class TotalRateResponse
	{
		[DataMember]
		public int Rate1;

		[DataMember]
		public int Rate2;

		[DataMember]
		public int Rate3;

		[DataMember]
		public int Rate4;

		[DataMember]
		public int Rate5;

		[DataMember]
		public int NotRated;
		
	}
}
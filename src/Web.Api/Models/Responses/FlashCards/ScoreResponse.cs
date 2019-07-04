using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.FlashCards
{
	[DataContract]
	public class ScoreResponse
	{
		[DataMember]
		public int One;

		[DataMember]
		public int Two;

		[DataMember]
		public int Three;

		[DataMember]
		public int Four;

		[DataMember]
		public int Five;

		[DataMember]
		public int NotViewed;
		
	}
}
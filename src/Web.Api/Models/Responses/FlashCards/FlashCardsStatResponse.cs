using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.FlashCards
{
	[DataContract]
	public class FlashCardsStatResponse
	{
		[DataMember]
		public int Total;

		[DataMember]
		public ScoreResponse ScoreResponse;
	}
}
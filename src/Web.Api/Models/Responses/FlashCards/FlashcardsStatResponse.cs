using System.Runtime.Serialization;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.Flashcards
{
	[DataContract]
	public class FlashcardsStatResponse
	{
		[DataMember]
		public int TotalFlashcardsCount;

		[DataMember(Name = "statistics")]
		public ScoreResponse ScoreResponse;
	}
}
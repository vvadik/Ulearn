using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.Flashcards
{
	[DataContract]
	public class UserFlashcardsStatistics
	{
		public UserFlashcardsStatistics()
		{
			FlashcardsStatistics = new FlashcardsStatistics();
		}

		[DataMember]
		public string UserId;

		[DataMember]
		public string UserName;

		[DataMember]
		public int TotalFlashcardsVisits;


		[DataMember]
		public FlashcardsStatistics FlashcardsStatistics;
	}
}
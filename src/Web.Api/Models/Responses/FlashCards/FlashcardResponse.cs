using System;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class FlashcardResponse
	{
		[DataMember]
		public Guid Id;

		[DataMember]
		public string Question;
		
		[DataMember]
		public string Answer;
		
		[DataMember]
		public string UnitTitle;
		
		[DataMember]
		public string Rate;
		
		[DataMember]
		public Guid UnitId;
	}
}
using System;
using System.Runtime.Serialization;
using Database.Models;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class FlashcardResponse
	{
		[DataMember]
		public string Id;

		[DataMember]
		public string Question;
		
		[DataMember]
		public string Answer;
		
		[DataMember]
		public string UnitTitle;
		
		[DataMember]
		public Rate Rate;
		
		[DataMember]
		public Guid UnitId;
	}
}
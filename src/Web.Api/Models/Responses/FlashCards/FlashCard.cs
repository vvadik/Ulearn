using System;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class FlashCard
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
		public string Status;
		
		[DataMember]
		public Guid UnitId;
	}
}
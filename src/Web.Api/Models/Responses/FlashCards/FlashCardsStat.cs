using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class FlashCardsStat
	{
		[DataMember]
		public int Total;

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
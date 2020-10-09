using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Models;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	public class ReviewInfo
	{
		
		[DataMember(Name="id")]
		public ShortUserInfo Author;

		[DataMember(Name="id")]
		public int StartLine;

		[DataMember(Name="id")]
		public int StartPosition;

		[DataMember(Name="id")]
		public int FinishLine;

		[DataMember(Name="id")]
		public int FinishPosition;

		[DataMember(Name="id")]
		public string Comment;

		[DataMember(Name="id")]
		public DateTime AddingTime;
	}
}
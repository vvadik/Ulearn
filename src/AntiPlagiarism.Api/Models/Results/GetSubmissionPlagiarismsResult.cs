using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AntiPlagiarism.Api.Models.Results
{
	[DataContract]
	public class GetSubmissionPlagiarismsResult : ApiSuccessResult
	{
		[DataMember(Name = "plagiarisms")]
		public List<Plagiarism> Plagiarisms { get; set; }
		
		[DataMember(Name = "tokens_positions")]
		public List<TokenPosition> TokensPositions { get; set; }
		
		// TODO (andgein): возвращать две отметки: 4 сигма и 6 сигма, но не больше 1 

		public GetSubmissionPlagiarismsResult()
		{
			Plagiarisms = new List<Plagiarism>();
		}
	}
}

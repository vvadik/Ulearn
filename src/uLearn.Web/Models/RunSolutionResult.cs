using System.Collections.Generic;
using uLearn.Web.Ideone;

namespace uLearn.Web.Models
{
	public class RunSolutionResult
	{
		public GetSubmitionDetailsResult ExecutionResult;
		public bool IsRightAnswer;
		public IEnumerable<string> AllAcceptedSolutions { get; set; }
		public string ExpectedOutput { get; set; }
	}
}
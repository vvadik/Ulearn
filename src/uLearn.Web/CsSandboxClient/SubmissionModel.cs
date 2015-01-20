namespace CsSandboxApi {
	public class SubmissionModel
	{
		public string Token { get; set; }

		public string Code { get; set; }

		public string Input { get; set; }

		public string HumanName { get; set; }

		public bool NeedRun { get; set; }
	}
}
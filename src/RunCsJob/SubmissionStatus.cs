namespace RunCsJob
{
	public enum SubmissionStatus
	{
		Done = 0,
		Waiting = 1,
		NotFound = 2,
		AccessDeny = 3,
		Error = 4,
		Running = 5,
		RequestTimeLimit = 6
	}
}

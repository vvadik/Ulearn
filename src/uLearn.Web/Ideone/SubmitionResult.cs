namespace uLearn.Web.Ideone
{
	public enum SubmitionResult
	{
		NotRunning = 0,
		CompilationError = 11,
		RuntimeError = 12,
		TimelimitExceeded = 13,
		Success = 15,
		MemoryLimitExceeded = 17,
		IllegalSystemCall = 19,
		InternalError = 20,
	}
}
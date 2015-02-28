namespace Selenium.UlearnDriverComponents.PageObjects
{
	public class RunResult
	{
		private readonly ResultType resultType;

		public RunResult(ResultType resultType)
		{
			this.resultType = resultType;
		}

		public ResultType GetResultType()
		{
			return resultType;
		}
	}

	public enum ResultType
	{
		ServiceError,
		CompileError,
		StyleError,
		Wa,
		WaNoDiff,
		Success
	}
}

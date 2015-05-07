namespace uLearn
{
	public class NullValidator : ISolutionValidator
	{
		public string FindFullSourceError(string userCode)
		{
			return null;
		}

		public string FindSyntaxError(string solution)
		{
			return null;
		}

		public string FindValidatorError(string userCode, string solution)
		{
			return null;
		}
	}
}
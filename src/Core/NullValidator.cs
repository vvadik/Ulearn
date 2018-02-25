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

		public string FindValidatorErrors(string userCode, string solution)
		{
			return null;
		}

		public string FindStrictValidatorErrors(string userCode, string solution)
		{
			return null;
		}
	}
}
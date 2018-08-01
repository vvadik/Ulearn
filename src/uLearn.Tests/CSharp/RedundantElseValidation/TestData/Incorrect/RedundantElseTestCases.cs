namespace uLearn.CSharp.RedundantElseValidation.TestData.Incorrect
{
	public class RedundantElseTestCases
	{
		public bool GetBoolValue()
		{
			if (true)
				return true;
			else
				return false;
		}
	}
}
using System;

namespace uLearn.CSharp.RedundantElseValidation.TestData.Correct
{
	public class RedundantElseTestCases
	{
		public bool ReturnInIfStatement()
		{
			if (true)
			{
				if (true)
					return true;
				var a = 1;
			}
			else
				return false;
		}
		
		public void ThrowInIfStatement()
		{
			if (true)
			{
				if (true)
					throw new Exception();
				var a = 1;
			}
			else
				ReturnInIfStatement();
		}
	}
}
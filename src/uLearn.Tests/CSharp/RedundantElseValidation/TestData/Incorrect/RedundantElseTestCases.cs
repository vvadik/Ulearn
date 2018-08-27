using System;

namespace uLearn.CSharp.RedundantElseValidation.TestData.Incorrect
{
	public class RedundantElseTestCases
	{
		public bool SingleReturnStatement()
		{
			if (true)
				return true;
			else
				return false;
		}
		
		public void SingleThrowStatement()
		{
			if (true)
				throw new Exception();
		}

		public bool ReturnInBlock()
		{
			if (true)
			{
				var a = 1;
				return true;
			}
			else
				return false;
		}
		
		public bool ThrowInBlock()
		{
			if (true)
			{
				var a = 1;
				throw new Exception();
			}
			else
				return false;
		}
		
		public bool ReturnInBlockIsNotLastStatement()
		{
			if (true)
			{
				return true;
				var a = 1;
			}
			else
				return false;
		}
		
		public bool ThrowInBlockIsNotLastStatement()
		{
			if (true)
			{
				throw new Exception();
				var a = 1;
			}
			else
				return false;
		}

		public int MultipleReturns()
		{
			if (true)
			{
				return 5;
			}
			else if (false)
			{
				return 6;
			}
		}

		public int ReturnAfterIfElseBlock()
		{
			int result = 1;
			if (true)
				return result;
			else
				result++;
			return result;
		}
	}
}
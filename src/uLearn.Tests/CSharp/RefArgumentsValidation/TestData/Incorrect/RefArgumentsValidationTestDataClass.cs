namespace uLearn.CSharp.RefArgumentsValidation.TestData.Incorrect
{
	internal class RefArgumentsValidationTestDataClass
	{
		public bool TryDoWork1(out int foo, out int workResult)
		{
			foo = 0;
			workResult = 0;
			return true;
		}

		public bool TryDoWork2(out int foo, ref int workResult)
		{
			foo = 0;
			return true;
		}

		public bool DoWork1(int foo, out int workResult)
		{
			workResult = 0;
			return true;
		}

		public void DoWork2(int foo, out int workResult)
		{
			workResult = 0;
		}

		public void TryDoWork(int foo, out int workResult)
		{
			workResult = 0;
		}

		public bool TryDoWork(out int workResult, int foo)
		{
			workResult = 0;
			return true;
		}

		public bool TryDoWork2(int foo, ref int workResult)
		{
			return true;
		}
	}
}

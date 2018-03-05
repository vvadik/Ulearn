namespace uLearn.CSharp.RefArgumentsValidation.TestData.Correct
{
	internal class RefArgumentsValidationTestDataClass
	{
		public void Foo() { }

		public void Foo(int y) { }

		public void Foo(int x, int y) { }

		public bool TryDoWork()
		{
			return true;
		}

		public bool TryDoWork(int foo, out int workResult)
		{
			workResult = 0;
			return true;
		}

		public bool TryDoWork2(int foo, out int workResult) => TryDoWork(foo, out workResult);
	}
}

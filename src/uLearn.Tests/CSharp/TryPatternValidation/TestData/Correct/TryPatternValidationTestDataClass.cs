namespace uLearn.CSharp.TryPatternValidation.TestData.Correct
{
	internal class TryPatternValidationTestDataClass
	{
		public void Foo() { }

		public void Foo(int y) { }

		public void Foo(int x, int y) { }

		public bool TryDoWork()
		{
			return true;
		}

		public bool TryDoWorkWithOut(int foo, out int workResult)
		{
			workResult = 0;
			return true;
		}

		public bool TryDoWorkWithOutFashionableSyntax(int foo, out int workResult) => TryDoWorkWithOut(foo, out workResult);

		public bool TryDoWorkWithRef(int foo, ref int workResult)
		{
			workResult = 0;
			return true;
		}

		public bool TryDoWorkWithRefFashionableSyntax(int foo, ref int workResult) => TryDoWorkWithRef(foo, ref workResult);
	}
}

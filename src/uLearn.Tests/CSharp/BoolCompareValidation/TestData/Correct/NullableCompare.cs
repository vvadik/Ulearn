namespace uLearn.CSharp.BoolCompareValidation
{
	public class NullableCompare
	{
		private TestClass testClass = new TestClass();

		public bool Compare(bool? a)
		{
			if (false == a)
			{
			}
			return a == true;
		}

		public bool Method(bool? b)
		{
			if (b ?? true)
			{
			}
			if (testClass?.str ?? true)
			{
			}
			return b ?? false;
		}
	}

	class TestClass
	{
		public bool? str;
	}
}
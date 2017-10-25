namespace uLearn.CSharp.BoolCompareValidation.TestData.Correct
{
	public class SimpleIf
	{
		public void Compare(int a, bool b)
		{
			if (a == 1)
			{
			}
			if (b)
			{
			}
		}

		bool A()
		{
			if (true)
				return true;
			else
				return false;
		}

		int B(int a, int v)
		{
			if (a == v)
				return a;
			else
				return v;
		}

		int X(int a, int v)
		{
			if (a != v && a > 0)
				return a;
			else
				return v;
		}

		int C(int a, int v)
		{
			return a == v ? a : v;
		}

		int D(int a, int v)
		{
			return a != v ? a : v;
		}
	}
}
namespace uLearn.CSharp.BoolCompareValidation.TestData.Incorrect
{
	public class TernaryOperatorWithBool
	{
		int A(bool a)
		{
			return true == a ? 1 : 0;
		}

		int C(bool a)
		{
			return true != a ? 1 : 0;
		}

		int D(bool a)
		{
			return false == a ? 1 : 0;
		}

		int E(bool a)
		{
			return false != a ? 1 : 0;
		}

		int F(bool a)
		{
			return a == true ? 1 : 0;
		}

		int G(bool a)
		{
			return a != true ? 1 : 0;
		}

		int T(bool a)
		{
			return a == false ? 1 : 0;
		}

		int Q(bool a)
		{
			return a != false ? 1 : 0;
		}

		int Y(bool a, bool b)
		{
			return a && b != false ? 1 : 0;
		}

		int Z(bool a, bool b, bool c)
		{
			return false != a && b || c ? 1 : 0;
		}
	}
}
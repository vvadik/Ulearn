namespace uLearn.CSharp.BoolCompareValidation.TestData.Incorrect
{
	public class IfWithBool
	{
		bool A(bool a)
		{
			if (a == true)
				return true;
			return false;
		}

		bool B(bool a)
		{
			if (a != true)
				return true;
			return false;
		}

		bool C(bool a)
		{
			if (a == false)
				return true;
			return false;
		}

		bool D(bool a)
		{
			if (a != false)
				return true;
			return false;
		}

		bool E(bool a)
		{
			if (true == a)
				return true;
			return false;
		}

		bool F(bool a)
		{
			if (false == a)
				return true;
			return false;
		}

		bool Q(bool a)
		{
			if (true != a)
				return true;
			return false;
		}

		bool W(bool a)
		{
			if (false != a)
				return true;
			return false;
		}
	}
}
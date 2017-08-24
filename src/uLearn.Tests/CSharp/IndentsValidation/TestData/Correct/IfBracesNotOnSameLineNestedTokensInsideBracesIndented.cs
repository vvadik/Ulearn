namespace Correct
{
	public class IfBracesNotOnSameLineNestedTokensInsideBracesIndented
	{
		void M0()
		{
			var a = new[]
			{
				1
			};
		}

		void M1()
		{
			var a = new[]
				{
					1
				};
		}

		void M2()
		{
			var a = new[]
					{
						1
					};
		}

		void M3
		(
			string b,
			string c
		)
		{
		}

		void M4
			(
				string b,
				string c
			)
		{
		}

		void M5
				(
					string b,
					string c
				)
		{
		}
	}
}
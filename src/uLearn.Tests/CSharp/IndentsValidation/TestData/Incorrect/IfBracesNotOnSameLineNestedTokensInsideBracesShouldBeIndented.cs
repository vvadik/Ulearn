namespace Incorrect
{
	public class IfBracesNotOnSameLineNestedTokensInsideBracesShouldBeIndented
	{ // все токены внутри фигурных скобок долджны иметь отступ > чем консистентный отступ токенов верхнего уровня вложенности внутри этих скобок
		void M0()
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

		void M21()
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
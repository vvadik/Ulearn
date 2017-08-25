namespace Incorrect
{
	public class IfBracesNotOnSameLineBracesShouldBeIndented
	{ // фигурные скобки долджны иметь отступ > чем отступ родителя
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
	}
}
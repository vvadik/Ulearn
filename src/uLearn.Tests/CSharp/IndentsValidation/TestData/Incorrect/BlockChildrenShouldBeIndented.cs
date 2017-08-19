namespace Incorrect
{
	public class BlockChildrenShouldBeIndented
	{
		public static void Main(string[] args)
		{ var a = 0;
			while (a++ < 5) { a--; }
			for (var i = 0; i < 5; i++) { a--; } while (a++ < 5) { a++; }
		}
	}
}
using System.Linq;

namespace Correct
{
	public class MultilineArgumentListIncreasesIndentationLength
	{
		public static void Main(string[] args)
		{
			new[] { 1, 2, 3 }.Select(
				i => i);

			new[] { 1, 2, 3 }.Select(
				i => new[] {i}
					.Select(a => a));

		}
}
}
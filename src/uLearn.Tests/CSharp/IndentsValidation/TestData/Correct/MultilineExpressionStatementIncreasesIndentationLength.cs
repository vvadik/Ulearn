using System;
using System.Linq;

namespace Correct
{
	public class MultilineExpressionStatementIncreasesIndentationLength
	{
		public static void Main(
			string[] args)
		{
			new[] { 1, 2, 3 }.Select(i => i);

			new[] { 1, 2, 3 }
				.Select(i => i);

			var a = new[] { 1, 2, 3 };
			a
				.Select(i => i);

			new[] { 1, 2, 3 }
				.Select(i => i)
				.Where(i => i < 2)
				.ToArray();

			new[] { 1, 2, 3 }.Select(i => i)
				.Where(i => i < 2)
				.ToArray();

			new[] { 1, 2, 3 }
					.Select(i => new[] { i }
						.Select(i1 => i1));

			new[] { 1, 2, 3 }.Select(i => i)
				.Where(i => i < 2)
					.ToArray();

			Console.WriteLine(); new[] { 1, 2, 3 }
				.Select(i => new[] { i });

			Console.WriteLine(); new[] { 1, 2, 3 }
				.Select(i => new[] { i }
					.Select(l => l));

			Console.WriteLine(); new[] { 1, 2, 3 }
				.Select(i => new[] { i }
						.Select(l => l));

			Console.WriteLine(); new[] { 1, 2, 3 }
				.Select(i => new[] { i }
					.Select(l => l));

			new[] { 1, 2, 3 }
				.Select(i => new[] { i }
					.Select(l => l));

			new[] { 1, 2, 3 }
				.Select(i => new[] { i }
						.Select(l => l)); new[] { 1, 2, 3 }
				.Select(p => p);

			new[] { 1, 2, 3 }
				.Select(i => new[] { i }
						.Select(l => l)); new[] { 1, 2, 3 }
					.Select(p => p);

			new[] { 1, 2, 3 }
				.Select(i => new[] { i }
						.Select(l => l)); new[] { 1, 2, 3 }
				.Select(p => new[] { p }
					.Select(g => g));

			new[] { 1, 2, 3 }
				.Select(i => new[] { i }
						.Select(l => l)); new[] { 1, 2, 3 }
					.Select(p => new[] { p }
						.Select(g => g));
		}
	}
}
using System.Collections.Generic;

namespace Correct
{
	public class ContentOfBraceTokensKeepsIndentationLength
	{
		private object a1 = new
		{
			e = 5,
			g = "asd"
		};

		private object a2 = new { e = 5, g = "asd" };

		public string Prop1 { get; set; }

		public string Prop2
		{
			get;
			set;
		}

		private int[] i1 = new[]
		{
			1,
			2,
			3
		};

		private int[] i2 = new[] { 1, 2, 3 };

		public List<string> Coll1 => new List<string>
		{
			"a",
			"b"
		};

		public List<string> Coll2 => new List<string> { "a", "b" };

		List<List<List<int>>> l = new List<List<List<int>>>
		{
			new List<List<int>>
			{
				new List<int>
				{
					1,
					2,
					3
				}
			}
		};

		Dictionary<int, int> d1 = new Dictionary<int, int>
		{
			{ 1, 2 },
			{ 2, 3 },
		};

		Dictionary<int, int> d2 = new Dictionary<int, int> { { 1, 2 }, { 2, 3 } };

		Dictionary<int, List<int>> dl1 = new Dictionary<int, List<int>>
		{
			{
				1, new List<int>
				{
					2,
					3
				}
			}
		};

		Dictionary<int, List<int>> dl2 = new Dictionary<int, List<int>>
		{
			{ 1, new List<int> { 2, 3 } }
		};
	}
}
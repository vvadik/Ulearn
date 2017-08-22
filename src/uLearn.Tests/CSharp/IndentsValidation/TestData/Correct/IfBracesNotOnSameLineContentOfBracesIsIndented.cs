using System.Collections.Generic;

namespace Correct
{
	public class IfBracesNotOnSameLineContentOfBracesIsIndented
	{
		private int b = 0;
		private int c = 0;

		private int d = 0;
		public static void Main3(string[] args)
		{
			var a = new {
				name = "asd"
			};
		}

		public static void Main4(string[] args)
		{
			for (var i = 0; i < 10; i++) { 
				var a = new {
					name = "asd"
				};
			}
		}
		private object a1 = new
		{
			e = 5,
			g = "asd"
		};
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

		public static void Main5(string[] args)
		{
			for (var i = 0; i < 10; i++)
			for (var j = 0; j < 10; j++)
			{
				var a = new
				{
					name = "asd"
				};
			}
		}

		public static void Main6(string[] args)
		{
			for (var i = 0; i < 10; i++)
				{
					var a = new
					{
						name = "asd"
					};
				}
		}

		public static void Main7(string[] args)
		{
			for (var i = 0; i < 10; i++)
			{
				for (var j = 0; j < 10; j++)
				{
					var a = new
					{
						name = "asd"
					};
				}
			}
		}
	}

	public enum EnumIndentsChildrenAlways
	{
		I_Am_Indented, Me_Too1,
			Me_Too,
				So_Am_I, And_I,
				And_I1
	}
	
	public interface InterfaceIndentsChildrenAlways
	{
		void I_Am_Indented(params object[] args); void O();
			void Me_Too(params object[] args); void I();
	}

	public struct StructureIndentsChildrenAlways
	{
		public static void I_Am_Indented(string[] args)
		{

		}
	}
}
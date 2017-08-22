using System.Collections.Generic;

namespace Incorrect
{
	public class ClassShouldIndentChildrenAlways
	{
	public static void I_Am_Not_Indented(string[] args)
		{

		}
	}

	public enum EnumShouldIndentChildrenAlways
	{
	I_Am_Not_Indented, Me_Too
	}

	public interface InterfaceShouldIndentChildrenAlways
	{
	void I_Am_Not_Indented(params object[] args); void Do();
	}

	public struct StructureShouldIndentChildrenAlways
	{
public static void I_Am_Not_Indented(string[] args)
		{

		}
	}

	public class IfBracesNotOnSameLineContentOfBracesShouldBeIndented
	{
		public static void Main2(string[] args)
		{
		var a = 0; var b = 0; var c = 0;
		}

		public static void Main3(string[] args)
		{ var a = 0; var b = 0; var c = 0;
		}

		public static void Main10(string[] args)
		{var a = 0; var b = 0; var c = 0;
		}

		public static void Main7(string[] args)
		{var a = 0;
		var b = 0; var c = 0;
		}

		private object a1 = new
		{
		e = 5,
		g = "asd"
		};

		private object a2 = new
		{
	e = 5,
	g = "asd"
		};

		public string Prop1
		{
		get;
		set;
		}

		public string Prop2
		{
	get; set;
		}

		private int[] i1 = new[]
		{
		1, 2, 3
		};

		private int[] i2 = new[]
		{
	1, 2, 3
		};

		public List<string> Coll1 => new List<string>
		{
		"a",
		"b"
		};

		public List<string> Coll2 => new List<string>
		{
	"a",
	"b"
		};
		public List<string> Coll5 => new List<string>
		{
		"a",
			"b"
		};
		public List<string> Coll6 => new List<string>
		{
			"a",
		"b"
		};

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

		Dictionary<int, int> d2 = new Dictionary<int, int>
		{
		{ 1, 2 },
			{
			2,
			3
			}
		};

		Dictionary<int, List<int>> dl = new Dictionary<int, List<int>>
		{
			{ 1, new List<int>
			{
				2, 3
			}
			}
		};
		List<List<List<int>>> l2 = new List<List<List<int>>>
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
	}
}
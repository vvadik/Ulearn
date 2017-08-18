using System.Collections.Generic;

namespace Correct.IfOpenBraceTokenHasEndOfLineTrivia
{
	public class CSharpSyntaxNodeIntendsChildren
	{
		private object b = new
		{
			e = 5,
			g = "asd"
		};

		private string prop;
		public string Prop1
		{
			get
			{
				return prop;
			}
			set
			{
				prop = value;
			}
		}

		public string Prop2
		{
			get;
			set;
		}

		private int[] m = new[]
		{
			1,
			2,
			3
		};

		public List<string> Coll => new List<string>
		{
			"a",
			"b"
		};

		public CSharpSyntaxNodeIntendsChildren()
		{
			var intendedVar = 0;
			int i = 0;
			foreach (var j in new[] { 0, 1, 2 })
			{
				intendedVar++;
			}

			for (i = 0; i < 5; i++)
			{
				i++;
			}

			while (++i < 5)
			{
				intendedVar = 1;
			}

			do
			{
				intendedVar = 1;
			} while (i++ < 10);
		}
	}
}
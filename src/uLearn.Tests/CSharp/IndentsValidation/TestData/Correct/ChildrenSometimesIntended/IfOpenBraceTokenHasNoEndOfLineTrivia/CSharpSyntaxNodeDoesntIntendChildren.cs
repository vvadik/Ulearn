using System.Collections.Generic;

namespace Correct.IfOpenBraceTokenHasNoEndOfLineTrivia
{
	public class CSharpSyntaxNodeDoesntIntendChildren
	{
		private string prop;
		public string Prop1 { get { return prop; } set { prop = value; } }
		public string Prop2 { get; set; } = "a";
		private int[] m = new[] { 1, 2, 3 };
		public List<string> Coll => new List<string> { "a", "b" };
		private object a = new { f = 1 };
	}
}
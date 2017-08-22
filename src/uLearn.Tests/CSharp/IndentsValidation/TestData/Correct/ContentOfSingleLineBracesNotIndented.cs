using System;

namespace Correct
{
	public class ClassWithoutMembers { }

	public class ContentOfSingleLineBracesNotIndented
	{
		private string prop;
		public string Prop
		{ 
			get { return prop; }
			set { prop = value; }
		}

		public string Prop1 { get; set; }

		public event Action Ev1
		{
			add { var a = 0; }
			remove { var a = 0; }
		}

		private int[] i2 = new[] { 1, 2, 3 };

		private object a2 = new { e = 5, g = "asd" };

		public void SingleLineBlock_IsOk()
		{
			var intendedVar = 0;
			int i = 0;
			foreach (var j in new[] { 0, 1, 2 }) { intendedVar++; }
			for (i = 0; i < 5; i++) { i++; }
			while (++i < 5) { intendedVar = 1; }
			do { intendedVar = 1; } while (i++ < 10);
			while (i++ < 10) { intendedVar++; }
			var f = new[] { 1 }; var k = new[] { 2 }; var p = new[] { 3 };
		}

		public void Ok() { }
	}
}
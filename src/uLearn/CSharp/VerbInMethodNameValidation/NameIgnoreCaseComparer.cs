using System;
using System.Collections.Generic;

namespace uLearn.CSharp
{
	public class NameIgnoreCaseComparer : IEqualityComparer<string>
	{
		public bool Equals(string x, string y)
		{
			return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
		}

		public int GetHashCode(string obj)
		{
			return base.GetHashCode();
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn
{
	public class ExpectedOutputAttribute : Attribute
	{
		public ExpectedOutputAttribute(string s)
		{
			Output = s;
		}

		public string Output { get; private set; }
	}
}

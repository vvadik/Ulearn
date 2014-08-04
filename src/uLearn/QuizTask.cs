using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace uLearn
{
	public class QuizTask
	{
		public string Condition;
		[XmlArray("Variables")]
		public Var[] Var;
		public string RightAnswer;
	}

	public class Var
	{
		public string V;
	}
}

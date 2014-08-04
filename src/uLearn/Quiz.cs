using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace uLearn
{
	[XmlRootAttribute("Quiz", IsNullable = false)]
	public class Quiz
	{
		[XmlAttribute]
		public string Title;
		[XmlAttribute]
		public string Id;

		[XmlArrayAttribute("Items")]
		public QuizBlock[] QuizBlocks;
	}
}

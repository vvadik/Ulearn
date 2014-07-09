using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class SolutionForTesting
	{
		public string Usings { get; private set; }
		public string Content { get; private  set; }
		private int IndexForInsert { get; set; }

		public SolutionForTesting(string usings, string content, int indexForInsert)
		{
			Usings = usings;
			Content = content;
			IndexForInsert = indexForInsert;
		}

		public string BuildSolution(string usersExercise)
		{
			return Usings + Content.Insert(IndexForInsert, usersExercise) + "\n}\n}";
		}
	}
}

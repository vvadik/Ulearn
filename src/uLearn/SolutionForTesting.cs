using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public class SolutionForTesting
	{
		private string Usings { get; set; }
		public string Content { get; private  set; }
		private int IndexForInsert { get; set; }

		public SolutionForTesting(string usings, string content, int indexForInsert)
		{
			Usings = usings;
			Content = content.Trim();
			IndexForInsert = indexForInsert;
		}

		public string BuildSolution(string usersExercise)
		{
			var countOfNLines = 0;
			for (var i = IndexForInsert; i < Content.Length; i++) //inserting of user exercise
			{
				if (Content[i] == '\n') countOfNLines++;
				if (countOfNLines == 2)
				{
					var a = Content.Remove(IndexForInsert, i - IndexForInsert);
					a = Usings + a.Insert(IndexForInsert, usersExercise) + "\n}";
					return a;
				}
			}
			return "error";
		}
	}
}

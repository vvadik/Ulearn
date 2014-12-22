using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn; 

namespace U19_FunctionalProgramming
{
	[Slide("Знакомство с LINQ", "bd310b69-df70-4051-9c57-742ae1e8fe61")]
	class S060_Знакомство_с_LINQ
	{
		//#video 8FrA-BnLUYY
		/*
		## Заметки по лекции
		*/

		public class Student
		{
			public string LastName { get; set; }
			public string Group { get; set; }
		}

		public class Program
		{
			public static void MainX()
			{
				var students = new List<Student>
            {
                new Student { LastName="Jones", Group="FT-1" },
                new Student { LastName="Adams", Group="FT-1" },
                new Student { LastName="Williams", Group="KN-1"},
                new Student { LastName="Brown", Group="KN-1"}
            };

				var result = new List<string>();
				foreach (var s in students)
					if (s.Group == "KN-1")
						result.Add(s.LastName);

				//или с помощью LINQ можно написать так:
				result = students.Where(z => z.Group == "KN-1").Select(z => z.LastName).ToList();
			}
		}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Жара", "{673C8A47-9560-4458-9BD9-A0C0B58466AA}")]
	public class S011_ListTask
	{
		/*
		Вася этим летом отдыхал на море. А вы?
		Также, он вел дневник погоды в течении 3х недель отпуска, и каждый день записывал температуру.
		Теперь ему интересны только те температуры, которые он считает тёплыми.
		Реализуйте функцию ```GetHighTemperatures```, которая возвращает список только самых теплых температур,
		отсортированных в порядке возрастания.
		Температура считается высокой, если её значение >= ```HighTemperature```.
		*/
		private const int HighTemperature = 20;

		[Hint("Методы сортировки присутствую в нестатических методах List")]
		[ExpectedOutput("20\n21\n22\n22\n23\n24\n25\n44\n45")]
		public static void Main()
		{
			List<int[]> weatherDiary = new List<int[]>
			{
				 new int[] {15, 18, 20, 22, 19, 25, -8 },
				 new int[] {1, 2, 3, 21, 22, 23, 24 },
				 new int[] {-10, -1, 15, 45, 5, 44, 8 }
			};
			List<int> highTemperatures = GetHighTemperatures(weatherDiary);
			foreach (var temperature in highTemperatures)
				Console.WriteLine(temperature);
		}

		[Exercise]
		private static List<int> GetHighTemperatures(List<int[]> temperatures)
		{
			var result = new List<int>();
			foreach (var week in temperatures)
			{
				foreach (var temperature in week)
				{
					if (temperature >= HighTemperature)
						result.Add(temperature);
				}
			}
			result.Sort();
			return result;
			/*uncomment
			...
			*/
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Приятные знакомства", "{9A4D5FF4-2331-4BEF-A3EE-88E54BEC88C8}")]
	public class S021_DictionaryTask
	{
		/*
		В отпуске Вася не тратил время зря, я заводил новые знакомства.
		Он знакомился с другими крутыми программистами, отдыхающими в его отеле, и записывал их номера телефонов.
		В его дневнике получилось много записей типа ```"{name}:{number}"```.
		Заметив, что многие именя повторяются, он решил сделать словарь,
		и каждому имени поставить в соответствие все номера телефонов, принадлежащие людям с таким именем.
		Вася уже написал функцию ```ReadPhonebook```, которая считывает его каракули из блокнота,
		тем самым облегчив вам работу. Помогите ему!
		*/

		[ExpectedOutput(@"Ваня: 11211,79436,800944
Саша: 792356,89023785,579235,9999
Паша: 34621,788222
Юра: 092358
Вася: 00000
Гена: 89237589")]
		public static void Main()
		{
			var phonebook = ReadPhonebook();
			
			Dictionary<string, List<string>> optimazedPhonebook = GetOptimazedPhonebook(phonebook);
			foreach (var record in optimazedPhonebook)
				Console.WriteLine("{0}: {1}", record.Key, string.Join(",", record.Value));
		}

		[HideOnSlide]
		private static List<string> ReadPhonebook()
		{
			return new List<string>
			{
				"Ваня:11211",
				"Саша:792356",
				"Паша:34621",
				"Юра:092358",
				"Саша:89023785",
				"Вася:00000",
				"Ваня:79436",
				"Саша:579235",
				"Гена:89237589",
				"Паша:788222",
				"Ваня:800944",
				"Саша:9999"
			};
		}

		[Exercise]
		private static Dictionary<string, List<string>> GetOptimazedPhonebook(List<string> phonebook)
		{
			var dict = new Dictionary<string, List<string>>();
			foreach (var record in phonebook)
			{
				var list = record.Split(':');
				if (!dict.ContainsKey(list[0]))
					dict[list[0]] = new List<string>();
				dict[list[0]].Add(list[1]);
			}
			return dict;
			/*uncomment
			return new Dictionary<string, List<string>>();
			*/
		}
	}
}

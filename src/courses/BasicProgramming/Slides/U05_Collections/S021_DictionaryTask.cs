using System;
using System.Collections.Generic;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U05_Collections
{
	[Slide("Приятные знакомства", "{9A4D5FF4-2331-4BEF-A3EE-88E54BEC88C8}")]
	public class S021_DictionaryTask : SlideTestBase
	{
		/*
		В отпуске Вася не тратил время зря, а заводил новые знакомства.
		Он знакомился с другими крутыми программистами, отдыхающими с ним в одном отеле, и записывал их номера телефонов.
		
		В его дневнике получилось много записей вида `<name>: <number>`.
		
		Чтобы искать телефоны было быстрее, он решил сделать словарь,
		в котором каждому имени поставить в соответствие все номера телефонов, принадлежащие людям с таким именем.
		
		Вася уже написал функцию `ReadPhonebook`, которая считывает его каракули из блокнота. Помогите ему сделать все остальное!
		*/

		[ExpectedOutput(@"Ваня: 11211, 79436, 800944
Саша: 792356, 89023785, 579235, 9999
Паша: 34621, 788222
Юра: 092358
Вася: 00000
Гена: 89237589")]
		public static void Main()
		{
			var phonebook = ReadPhonebook();
			
			Dictionary<string, List<string>> optimizedPhonebook = GetOptimizedPhonebook(phonebook);
			foreach (var record in optimizedPhonebook)
				Console.WriteLine("{0}: {1}", record.Key, string.Join(", ", record.Value.ToArray()));
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
		[Hint("Проверяйте наличие ключа в словаре перед добавлением")]
		[Hint("Вспомните, что делает метод `Split` у строки")]
		private static Dictionary<string, List<string>> GetOptimizedPhonebook(List<string> phonebook)
		{
			var dictionary = new Dictionary<string, List<string>>();
			foreach (var record in phonebook)
			{
				var list = record.Split(':');
				var name = list[0];
				var phone = list[1];
				if (!dictionary.ContainsKey(name))
					dictionary[name] = new List<string>();
				dictionary[name].Add(phone);
			}
			return dictionary;
			/*uncomment
			var dictionary = new Dictionary<string, List<string>>();
			...
			return dictionary;
			*/
		}
	}
}

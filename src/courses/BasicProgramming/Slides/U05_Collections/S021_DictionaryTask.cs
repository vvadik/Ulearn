using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U05_Collections
{
	[Slide("Полезные знакомства", "{9A4D5FF4-2331-4BEF-A3EE-88E54BEC88C8}")]
	public class S021_DictionaryTask : SlideTestBase
	{
		/*
		В отпуске Вася не тратил время зря, а заводил новые знакомства.
		Он знакомился с другими крутыми программистами, отдыхающими с ним в одном отеле, и записывал их email-ы.
		
		В его дневнике получилось много записей вида `<name>:<email>`.
		
		Чтобы искать записи было быстрее, он решил сделать словарь,
		в котором по двум первым буквам имени можно найти все записи его дневника.
		
		Вася уже написал функцию `GetContacts`, которая считывает его каракули из блокнота. Помогите ему сделать все остальное!
		*/

		[ExpectedOutput(@"
Ва: Ваня:v@mail.ru, Вася:vasiliy@gmail.com, Ваня:ivan@grozniy.ru, Ваня:vanechka@domain.com
Са: Саша:sasha1995@sasha.ru, Саша:alex@nd.ru, Саша:alexandr@yandex.ru, Саша:a@lex.ru
Па: Паша:p@p.ru, Паша:pavel.egorov@urfu.ru
Юр: Юрий:dolg@rukiy.ru
Ге: Гена:genadiy.the.best@inbox.ru
Ы: Ы:nobody@nowhere.no")]
		[HideOnSlide]
		public static void Main()
		{
			List<string> contacts = GetContacts();
			
			Dictionary<string, List<string>> optimizedPhonebook = OptimizeContacts(contacts);

			foreach (var record in optimizedPhonebook)
				Console.WriteLine("{0}: {1}", record.Key, string.Join(", ", record.Value.ToArray()));
		}

		[HideOnSlide]
		private static List<string> GetContacts()
		{
			return new List<string>
			{
				"Ваня:v@mail.ru",
				"Саша:sasha1995@sasha.ru",
				"Паша:p@p.ru",
				"Юрий:dolg@rukiy.ru",
				"Саша:alex@nd.ru",
				"Вася:vasiliy@gmail.com",
				"Ваня:ivan@grozniy.ru",
				"Саша:alexandr@yandex.ru",
				"Гена:genadiy.the.best@inbox.ru",
				"Паша:pavel.egorov@urfu.ru",
				"Ваня:vanechka@domain.com",
				"Саша:a@lex.ru",
				"Ы:nobody@nowhere.no"
			};
		}

		[Exercise]
		[Hint("Разбить запись на имя и email вам поможет уже знакомый метод `Split` у строки")]
		[Hint("Проверяйте наличие ключа в словаре перед добавлением")]
		private static Dictionary<string, List<string>> OptimizeContacts(List<string> contacts)
		{
			var dictionary = new Dictionary<string, List<string>>();
			foreach (var record in contacts)
			{
				var name = record.Split(':').First();
				name = name.Substring(0, Math.Min(name.Length, 2));
				if (!dictionary.ContainsKey(name))
					dictionary[name] = new List<string>();
				dictionary[name].Add(record);
			}
			return dictionary;
			/*uncomment
			var dictionary = new Dictionary<string, List<string>>();
			


			return dictionary;
			*/
		}
	}
}

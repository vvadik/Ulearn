using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace SharpLessons.Linqed
{
	[TestFixture]
	public class LookupExercise
	{
		/*

		##Задача: Обратный индекс

		Обратный индекс — это структура данных, часто использующаяся в задачах 
		полнотекстового поиска нужного документа в большой базе документов.

		По своей сути обратный индекс напоминает индекс в конце бумажных энциклопедий, 
		где для каждого ключевого слова указан список страниц, где оно встречается.

		Вам требуется по списку документов построить обратный индекс.

		Документ определен так:
		*/

		[Sample]
		public class Document
		{
			public int Id;
			public string Text;
		}

		/*
		Обратный индекс в нашем случае — это словарь `IDictionary<string, HashSet<int>>`, 
		ключем в котором является слово, а значением — хэштаблица, содержащая идентификаторы
		всех документов, содержащих это слово.
		*/


		[Exercise(SingleStatement = true)]
		[Hint("Сегодня никаких подсказок!")]
		[Hint("Да, задача сложная, но тем не менее подсказок не будет!")]
		[Hint("Ну правда, пора научиться решать подобные задачи без подсказок!")]
		public IDictionary<string, HashSet<int>> BuildInvertedIndex(Document[] documents)
		{
			return
				documents
					.SelectMany(doc =>
						Regex.Split(doc.Text, @"\W+")
							.Where(word => word != "")
							.Select(word => Tuple.Create(word.ToLower(), doc))
					)
					.GroupBy(wordDoc => wordDoc.Item1)
					.ToDictionary(
						group => group.Key,
						group => new HashSet<int>(group.Select(wordDoc => wordDoc.Item2.Id)));
			// ваш код
		}

		[Test]
		public void TestInvertedIndex()
		{
			Document[] docs =
			{
				new Document {Id = 1, Text = "Hello world!"},
				new Document {Id = 2, Text = "World, world, world... Just words..."},
				new Document {Id = 3, Text = "Words — power"},
				new Document {Id = 4, Text = ""},
			};
			var index = BuildInvertedIndex(docs);

			Assert.That(index["world"], Is.EquivalentTo(new[] {1, 2}));
			Assert.That(index["words"], Is.EquivalentTo(new[] {2, 3}));
			Assert.That(index["power"], Is.EquivalentTo(new[] {3}));
			Assert.That(!index.ContainsKey(""));
		}
	}
}
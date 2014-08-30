using System.Collections.Generic;
using System.Linq;

namespace uLearn.Courses.Linq.Slides
{
	[Slide("Введение", "{3446FAB2-15DF-4045-AB40-ABC1F3DC87C8}")]
	public class S010_Intro
	{
		/*
		`LINQ` — это встроенный в C# механизм для удобной работы с коллекциями.

		Большинство алгоритмов, которые на менее развитых языках принято писать с помощью циклов и условных операторов, 
		более компактно и красиво выражаются с помощью примитивов `LINQ`.

		Посмотрите на код поиска всех новых писем в классическом стиле:
		*/

		public int[] GetNewLettersIds_ClassicWay()
		{
			var res = new List<int>();
			for(int i=0; i<letters.Length; i++)
			{
				if (letters[i].IsNew)
					res.Add(letters[i].Id);
			}
			return res.ToArray();
		}

		/*
		Похожий код каждому программисту приходилось писать ни один раз. 
		
		А вот версия решения той же задачи с помощью `LINQ`:
		*/

		public IEnumerable<int> GetNewLettersIds_LinqWay()
		{
			return letters.Where(l => l.IsNew).Select(l => l.Id);
		}

		/*
		Всего одна строчка! Короткая и понятная!

		Лекционные слайды познакомят вас с основными возможностями `LINQ`.
		Однако главная ценность этого курса — тщательно подобранный набор упражнений.
		Они максимально близки к реальным задачам, встречающимся на практике, и хорошо демонстрирую выразительные возможности `LINQ`.
		
		Если вы уже имеете представление о `LINQ`, можете пропускать лекционные слайды, но прорешать упражнения.
		Некоторые из них могут открыть вам новый взгляд на давно знакомые задачи.
		*/

		[HideOnSlide]
		private readonly Letter[] letters = new Letter[0];

		[HideOnSlide]
		public class Letter
		{
			public bool IsNew { get; set; }
			public int Id { get; set; }
		}
	}
}
using System;
using NUnit.Framework;

namespace uLearn.Courses.Linq.Slides
{
	[Id("Work_Tuples")]
	[Title("Работа с кортежами")]
	[TestFixture]
	public class Tuples
	{
		/*

		*/

		[ShowBodyOnSlide]
		[Test]
		public void Test1()
		{
			var tuple = new Tuple<int, string, bool>(42, "abc", true);

			// Сокращенная запись с автоматическим выводом типов кортежа:
			var tuple2 = Tuple.Create(42, "abc", true);

			// Доступ к компонентам кортежа:
			Assert.That(tuple.Item1, Is.EqualTo(42));
			Assert.That(tuple.Item2, Is.EqualTo("abc"));
			Assert.That(tuple.Item3, Is.EqualTo(true));

			//Переопределенный ToString 
			Assert.That(tuple.ToString(), Is.EqualTo("(42, abc, True)"));

			// Переопределенный Equals сравнивает значения компонент кортежа
			Assert.That(tuple, Is.EqualTo(tuple2));
		}
	}
}
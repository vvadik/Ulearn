using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SharpLessons.Linqed
{
	[TestFixture]
	public class AnonymousTypes
	{
		/*

		##Анонимные типы
		
		В .NET есть возможность создавать объекты типов, не объявленных ранее.
		В примере ниже создается объект person, анонимного типа.
		В таких случаях компилятор создает тип за программиста, основываясь на том,
		какие поля были использованы при создании объекта.

		Так в примере ниже будт создан анонимный тип с тремя полями: name, age и isMarried.

		*/

		[Sample]
		[Test]
		public void Test1()
		{
			var person = new {name = "Pavel", age = 32, isMarried = true};
			Assert.That(person.isMarried);
			Assert.That(person.name, Is.EqualTo("Pavel"));
			Assert.That(person.age, Is.EqualTo(32));
			Assert.That(person, Is.EqualTo(new {name = "Pavel", age = 32, isMarried = true}));
		}

		/*
		Последняя проверка демонстрирует, что объекты анонимных типов сравниваются не по ссылкам, а по значениям.

		Анонимные типы — самостоятельная и независимая возможность языка C#, 
		однако иногда их очень удобно использовать в Linq запросах. Это демонстрирует следующий пример:
		*/

		[Sample]
		[Test]
		public void Test2()
		{
			string[] names = {"Anna", "Pavel", "Irina", "Alexander"};

			var shortNames = names
				.Select(name => new {name, len = name.Length})
				.Where(nameInfo => nameInfo.len <= 4)
				.Select(nameInfo => nameInfo.name);

			Assert.That(shortNames, Is.EqualTo(new[] {"Anna"}).AsCollection);
		}
	}
}
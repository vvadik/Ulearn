using System;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("Классы", "76F6899B-9637-4A73-BB15-D33E2C3256AB")]
	public class S010_Classes
	{
		//#video ETt0qj1SAx8
		/*
		## Заметки по лекции
		*/

		//Класс — это совокупность полей и методов (с методами разберемся далее)
		public class Student
		{
			//Так объявляется поле класса.
			//Оно не статическое, т.е. не является глобальной переменной. 
			//Разницу между статическими и динамическими полями мы разберем позже.
			//С ключевым словом public мы также разберемся позже
			public int Age;
			public string FirstName;
			public string LastName;
		}

		public class Program
		{
			//мы можем создать массив из Student, потому что Student — это такой же тип, как string или int
			static Student[] students;

			//Этот тип можно использовать в аргументах метода. 
			//Если мы захотим добавить новое поле в Student, не придется переписывать заголовок метода
			static void PrintStudent(Student student)
			{
				Console.WriteLine("{0,-15}{1}", student.FirstName, student.LastName);
			}

			static void Main()
			{
				students = new Student[2];

				//Классы — это ссылочные типы, поэтому их нужно инициализировать.
				//Можно сделать собственный тип-значение, но это мы рассмотрим позже.
				students[0] = new Student();
				students[0].FirstName = "John";
				students[0].LastName = "Jones";
				students[0].Age = 19;
				students[1] = new Student();
				students[1].FirstName = "William";
				students[1].LastName = "Williams";
				students[1].Age = 18;

				//Можно делать это с помощью сокращенной инициализации — это то же самое
				students = new[]
				{
					new Student { LastName = "Jones", FirstName = "John" },
					new Student { LastName = "Williams", FirstName = "William" }
				};
			}
		}
		/*
		### Карта памяти при работе с классами:
		![Карта памяти](classes.png)
		*/
	}
}
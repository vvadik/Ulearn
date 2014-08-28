using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace uLearn.Courses.BasicProgramming.Slides.U05_Collections
{
	[Slide("Файлы и каталоги", "{E7EA11C8-0AAA-425D-97DF-96923C5A1609}")]
	public class S070_FilesAndDirectories
	{
		//#video GknsLKazNIw

		/*
		## Заметки по лекции
		*/

		public static void Main()
		{
			// Запись текста в файл:
			File.WriteAllText("test.txt", "Hello, world!");

			// Путь относительно "текущей директории", которую можно узнать так:
			Console.WriteLine(Environment.CurrentDirectory);
			// Обычно это директория, в которой была запущена ваша программа

			// А размещение запущенного exe-файла можно узнать так:
			Console.WriteLine(Assembly.GetExecutingAssembly().Location);

			// Сфорировать абсолютный путь по относительному можно так:
			Console.WriteLine(Path.Combine(Environment.CurrentDirectory, "test.txt"));

			
			File.WriteAllLines("test1.txt", new []{"Hello", "world"});

			File.WriteAllBytes("text.dat", new byte[10240]);

			// Чтение данных из файла
			string text = File.ReadAllText("text.txt");
			string[] lines = File.ReadAllLines("text1.txt");
			byte[] bytes = File.ReadAllBytes("text.dat");

			// Все файлы в текущей директории (точка в пути означает текущую директорию)
			foreach (var file in Directory.GetFiles("."))
				Console.WriteLine(file);
		}
	}
}

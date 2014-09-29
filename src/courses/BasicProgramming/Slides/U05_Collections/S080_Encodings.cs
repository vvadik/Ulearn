using System;
using System.IO;
using System.Text;

namespace uLearn.Courses.BasicProgramming.Slides.U05_Collections
{
	//#video YxFheXv6Vv8
	/*
	## Заметки по лекции
	*/
	[Slide("Кодировка и работа с файлами", "{8E38E049-311E-45CD-B546-99FE1EC2C9D6}")]
	public class S080_Encodings
	{
		//Когда мы говорим о записи текста в файл, возникают кодировки
		//Кодировка - это то, что превращает символ (абстрактную сущность) в байты (конкретные числа от 0 до 255)
		//Вне кодировки, никакой связи между символами и байтами нет!
		static void WriteAndRead(string text, Encoding encoding)
		{
			Console.WriteLine("{0}, encoding {1}", text, encoding.EncodingName);
			//Так можно записать в файл некий текст
			//Альтернативы - WriteAllLines (записывает массив строк) или WriteAllBytes (массив байт)
			File.WriteAllText("temp.txt", text, encoding);

			//Так можно прочитать массив байт
			//Альтернативы интуитивно понятны
			var bytes = File.ReadAllBytes("temp.txt");
			foreach (var e in bytes)
				Console.Write("  {0} ", (char)e);
			Console.WriteLine();
			foreach (var e in bytes)
				Console.Write("{0:D3} ", e);
			Console.WriteLine();

			//В С# есть явное преобразование типа byte в char. 
			//Это — наследие прежних эпох, когда кодировка была только одна.
			//Злоупотреблять этим не стоит

		}


		public static void Main()
		{

	
			//Английский язык и базовые символы одинаковы во всех кодировках
			//Однако, при сохранении текста в кодировке UTF добавляется специальный маркер файла,
			//по которому текстовые редакторы определяют кодировку текста
			WriteAndRead("Hello!", Encoding.ASCII);
			WriteAndRead("Hello!", Encoding.UTF8);


			//Русские буквы нельзя сохранять в ASCII
			WriteAndRead("Привет!", Encoding.ASCII);

			//Можно попробовать в кодировке локали, но этого лучше не делать:
			//В этом случае файл не самодостаточен, для его прочтения нужно знать
			//какая кодировка у вас в локали
			WriteAndRead("Привет!", Encoding.Default);

			//UTF-8 - лучшее решение!
			//Русские буквы кодируются в ней двумя байтами
			WriteAndRead("Привет!", Encoding.UTF8);

			//А китайские иероглифы - уже тремя
			WriteAndRead("你好!", Encoding.UTF8);
		}

		/*
		* [Таблица всех символов Unicode](http://unicode-table.com/en/)
		* Статья Джоела Скольски [Что каждый разработчик должен знать о Unicode](http://local.joelonsoftware.com/wiki/%D0%90%D0%B1%D1%81%D0%BE%D0%BB%D1%8E%D1%82%D0%BD%D1%8B%D0%B9_%D0%9C%D0%B8%D0%BD%D0%B8%D0%BC%D1%83%D0%BC,_%D0%BA%D0%BE%D1%82%D0%BE%D1%80%D1%8B%D0%B9_%D0%9A%D0%B0%D0%B6%D0%B4%D1%8B%D0%B9_%D0%A0%D0%B0%D0%B7%D1%80%D0%B0%D0%B1%D0%BE%D1%82%D1%87%D0%B8%D0%BA_%D0%9F%D1%80%D0%BE%D0%B3%D1%80%D0%B0%D0%BC%D0%BC%D0%BD%D0%BE%D0%B3%D0%BE_%D0%9E%D0%B1%D0%B5%D1%81%D0%BF%D0%B5%D1%87%D0%B5%D0%BD%D0%B8%D1%8F_%D0%9E%D0%B1%D1%8F%D0%B7%D0%B0%D1%82%D0%B5%D0%BB%D1%8C%D0%BD%D0%BE_%D0%94%D0%BE%D0%BB%D0%B6%D0%B5%D0%BD_%D0%97%D0%BD%D0%B0%D1%82%D1%8C_%D0%BE_Unicode_%D0%B8_%D0%9D%D0%B0%D0%B1%D0%BE%D1%80%D0%B0%D1%85_%D0%A1%D0%B8%D0%BC%D0%B2%D0%BE%D0%BB%D0%BE%D0%B2)
		*/
	}
}
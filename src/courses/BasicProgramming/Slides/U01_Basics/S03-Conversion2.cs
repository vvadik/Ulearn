using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Преобразования типов 2", "{2B6E076C-4617-4322-9BDE-C44A1D4F394F}")]
	public class S03_Conversion2
	{
		/*
		Вася регулярно получает за красивые глазки от кого-нибудь по ```amount``` [биткоинов](http://ru.wikipedia.org/wiki/Bitcoin).
		
		Вася хочет знать сколько биткоинов у него уже накопилось. А чтобы не мелочиться, хочет округлить сумму до ближайшего целого.
		
		Он написал для этого программу, но что-то с ней не так... Помогите ему исправить все ошибки.
		*/

		[Exercise]
		[ExpectedOutput("67")]
		public static void Main()
		{
			double amount = 1.11;
			int peopleCount = 60; // количество человек, которые подарили Васе amount биткоинов каждый.
			double totalMoney = Math.Round(amount * peopleCount);
			Console.WriteLine(totalMoney);
			/*uncomment
			double amount = 1.11; //количество биткоинов от одного человека
			int peopleCount = 60; // количество человек
			double totalMoney = (int) amount*peopleCount; // ← исправьте ошибку в этой строке
			Console.WriteLine(totalMoney);
			*/
		}
	}
}

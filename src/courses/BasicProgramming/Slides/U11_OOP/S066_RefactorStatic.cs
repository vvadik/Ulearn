using System;
using System.Globalization;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("Рефакторинг статического класса", "A32D7A49335A4394852D10692E241EFB")]
	public class S045_RefactorStatic : SlideTestBase
	{
		/*
		Как вы уже догадались... Сделайте так, чтобы код заработал!
		Для этого сделайте класс SuperBeautyImageFilter не статическим.
		*/

		[ExpectedOutput("Processing Paris.jpg with parameter 0.4")]
		[Hint("Сделайте методы SuperBeautyImageFilter не статическими")]
		[Hint("Не забудьте убрать модификатор static у самого класса")]
		public static void Main()
		{
			var filter = new SuperBeautyImageFilter();
			filter.ImageName = "Paris.jpg";
			filter.GaussianParameter = 0.4;
			filter.Run();
		}

		[ExcludeFromSolution]
		[HideOnSlide]
		public class SuperBeautyImageFilter
		{
			public string ImageName;
			public double GaussianParameter;
			public void Run()
			{
				Console.WriteLine("Processing {0} with parameter {1}", 
					ImageName, 
					GaussianParameter.ToString(CultureInfo.InvariantCulture));
				//do something useful
			}
		}
		/*uncomment
		public static class SuperBeautyImageFilter
		{
			public static string ImageName;
			public static double GaussianParameter;
			public static void Run()
			{
				Console.WriteLine("Processing {0} with parameter {1}", 
					ImageName, 
					GaussianParameter.ToString(CultureInfo.InvariantCulture));
				//do something useful
			}
		}
		*/

	}
}
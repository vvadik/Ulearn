using System;
using System.Globalization;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("Создание классов", "B0529FF452D14462865B89920B240F57")]
	public class S016_CreateClass : SlideTestBase
	{
		/*
		Сделайте так, чтобы код заработал!
		Для этого создайте недостающие классы City и GeoLocation.
		*/

		[ExpectedOutput("I love Ekaterinburg located at (60.35, 56.5)")]
		static void Main()
		{
			var city = new City();
			city.Name = "Ekaterinburg";
			city.Location = new GeoLocation();
			city.Location.Latitude = 56.50;
			city.Location.Longitude = 60.35;
			Console.WriteLine("I love {0} located at ({1}, {2})", 
				city.Name, 
				city.Location.Longitude.ToString(CultureInfo.InvariantCulture),
				city.Location.Latitude.ToString(CultureInfo.InvariantCulture));
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		class GeoLocation
		{
			public double Latitude;
			public double Longitude;
		}
		[HideOnSlide]
		[ExcludeFromSolution]
		class City
		{
			public GeoLocation Location;
			public string Name;
		}
	}
}
using System;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("Создание классов", "B0529FF452D14462865B89920B240F57")]
	public class S016_CreateClass : SlideTestBase
	{

		[ExpectedOutput("I love Yekaterinburg")]
		static void Main()
		{
			var city = new City();
			city.Name = "Yekaterinburg";
			city.FoundationDate = new DateTime(1723, 11, 18);
			city.Location = new GeoLocation();
			city.Location.Latitude = 56.50;
			city.Location.Longitude = 60.35;
			Console.WriteLine("I love " + city.Name);
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
			public DateTime FoundationDate;
		}
	}
}
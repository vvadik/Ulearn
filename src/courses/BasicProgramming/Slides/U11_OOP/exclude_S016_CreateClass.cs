//using System;

//namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
//{
//    [Slide("Создание классов", "{06AA4E3E-C1F8-4895-BA1F-B7D5DF22BB28}")]
//    public class exclude_S016_CreateClass : SlideTestBase
//    {

//        [ExpectedOutput("I love Yekaterinburg!")]
//        static void Main()
//        {
//            var city = new City();
//            city.Name = "Yekaterinburg";
//            city.FoundationDate = new DateTime(1723, 11, 18);
//            city.Location = new GeoLocation();
//            city.Location.Latitude = 56.50;
//            city.Location.Longitude = 60.35;
//            Console.WriteLine("I love " + city.Name);
//        }

//        [HideOnSlide]
//        [ExcludeFromSolution]
//        class GeoLocation
//        {
//            public double Latitude;
//            public double Longitude;
//        }
//        [HideOnSlide]
//        [ExcludeFromSolution]
//        class City
//        {
//            public GeoLocation Location;
//            public string Name;
//            public DateTime FoundationDate;
//        }
//    }
//}
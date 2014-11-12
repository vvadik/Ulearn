using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
    [Slide("Создание класса", "{1612E51D-8DFB-4E2F-B520-351B299AA3F2}")]
    public class S015_CreateClass : SlideTestBase
    {
        /* Создайте класс Town так, чтобы код скомпилировался
         */
        [ExpectedOutput("I love Yekaterinburg!")]
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

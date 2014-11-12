//using System;
//
//namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
//{
//    [Slide("Создание классов", "{06AA4E3E-C1F8-4895-BA1F-B7D5DF22BB28}")]
//    public class S045_CreateExtension : SlideTestBase
//    {
//        /*
//         
//         Сделайте так, чтобы заработало. 
//         */
//
//        [ExpectedOutput("")]
//        [Hint("Класс SuperBeautyImageFilter должен перестать быть статическим")]
//        static void Main()
//        {
//            var filter = new SuperBeautyImageFilter();
//            filter.ImageName = "Paris.jpg";
//            filter.GaussianParameter = 0.4;
//            filter.Run();
//        }
//
//        [HideOnSlide]
//        [ExcludeFromSolution]
//        public class SuperBeautyImageFilter
//        {
//            //это надо как-то убрать, и вставить то, что ниже. Короче, это тоже непонятно, как делать.
//            public string ImageName;
//            public double GaussianParameter;
//            public void Run()
//            {
//
//            }
//
//            //в окошке редактирования должно быть это:
//            public static string ImageName;
//            public static double GaussianParameter;
//            public static void Run()
//            {
//                //do something useful
//            }
//        }
//
//    }
//}
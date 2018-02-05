using System;
using System.Text;
using uLearn;
using System.Linq;

namespace CS2
{
    [Slide("Упражнение на параметры по умолчанию", "{D6165AFA-81B2-452F-9E61-78D3580E0C27}")]
    public class S065_Operators 
    {
        /*
        Отрефакторьте класс MyFile, чтобы в нем остался только один метод
        */

        [ExpectedOutput(@"
Use encoding System.Text.UTF8Encoding
Use encoding System.Text.UTF32Encoding
1")]
        
        public static void Main()
        {
            var str1 = MyFile.ReadAll("test.txt");
            var str2 = MyFile.ReadAll("test.txt", Encoding.UTF32);
            Console.Write(MethodsCount());
        }

        [HideOnSlide]
        static int MethodsCount()
        {
            return typeof(MyFile).GetMethods().Where(z => z.IsStatic).Count();
        }

        [HideOnSlide]
        [ExcludeFromSolution]
        class MyFile
        {
            public static string ReadAll(string filename, Encoding enc=null)
            {
                if (enc == null) enc = Encoding.UTF8;
                Console.WriteLine("Use encoding " + enc.ToString());
                return null;
            }
        }

        /*uncomment
        class MyFile
        {
            public static string ReadAll(string filename, Encoding enc)
            {
                Console.WriteLine("Use encoding " + enc.ToString());
                return null;
            }
          
            public static string ReadAll(string filename)
            {
                return ReadAll(filename, Encoding.UTF8);
            }
        }*/
    }
}
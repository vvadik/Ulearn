//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using uLearn.CSharp;

//namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
//{
//    [Slide("Список директорий", "{24E64255-D0E2-418F-A86F-122D67117355}")]
//    class S055_FileInfo : SlideTestBase
//    {
//        /*
//         Вы пишете духовную операционную систему "Оконце, версия 0.99", и модуль к этой системе,
//         отвечающий за составление медиа-альбомов пользователя. По данному вам списку файлов, составьте 
//         список всех директорий, которые содержат файлы с расширением .mp3 и .wav. 
         
//         Каждую директорию нужно выводить только один раз, порядок значения не имеет.
//         */
         
//        [ExpectedOutput("Любо!")]
//        [HideOnSlide]
//        public static void Main()
//        {
//            if (!Check(@"A\1.mp3", @"B\2.mp3", @"C\3.mp3")) return;
//            if (!Check(@"A\1.mp3", @"B\2.wav")) return;
//            if (!Check(@"A\B\1.mp3", @"A\2.mp3", @"B\3.mp3")) return;
//            if (!Check(@"A\1.mp3", @"A\2.mp3")) return;
//            if (!Check()) return;
//            if (!Check(@"A\1.txt")) return;
//            Console.WriteLine("Любо!");
//        }

//        [HideOnSlide]
//        static bool Check(params string[] fnames)
//        {
//            var files = fnames.Select(z => new FileInfo(z)).ToList();
//            var dirsExpected = GetAlbumsEtalon(files);
//            var dirsActual = GetAlbums(files);
//            var errorMesage = string.Format("Не любо! При анализе списка файлов\n{0}\nдан ответ:\n{1}\nа правильный ответ:\n{2}\n",
//                fnames.Aggregate((a, b) => a + "\n" + b),
//                dirsActual.Select(z => z.FullName).Aggregate((a, b) => a + "\n" + b),
//                dirsExpected.Select(z => z.FullName).Aggregate((a, b) => a + "\n" + b));

//            if (dirsActual.Count!=dirsExpected.Count)
//            {
//                Console.WriteLine(errorMesage);
//                return false;
//            }
//            dirsActual = dirsActual.OrderBy(z => z.FullName).ToList();
//            dirsExpected = dirsExpected.OrderBy(z => z.FullName).ToList();
//            for (int i=0;i<dirsActual.Count;i++)
//                if (dirsActual[i].FullName != dirsExpected[i].FullName)
//                {
//                    Console.WriteLine(errorMesage);
//                    return false;
//                }
//            return true;
//        }

//        [HideOnSlide]
//        static List<DirectoryInfo> GetAlbumsEtalon(List<FileInfo> files)
//        {
//            return files
//                .Where(z => z.Extension == ".mp3" || z.Extension == ".wav")
//                .Select(z => z.Directory)
//                .Distinct()
//                .ToList();
//        }
//        [Exercise]
//        public static List<DirectoryInfo> GetAlbums(List<FileInfo> files)
//        {
//            return GetAlbumsEtalon(files);
//            /*uncomment
//            ...
//            */
//        }
//    }
//}

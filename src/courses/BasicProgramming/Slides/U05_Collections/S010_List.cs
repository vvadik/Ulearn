using System;
using System.Collections.Generic;
using uLearn;

namespace Slide01
{
	//#video KwFrLY_aRyo
	/*
	## Заметки по лекции
	*/
	[Slide("Списки", "{D203BFDE-49E4-45AB-B9EE-DB11975F99D0}")]
    public class S010_List
    {
        static void Main()
        {
            //Длину массива нельзя изменить
            //Иногда хочется, чтобы массив динамически рос, потому что мы не знаем заранее,
            //сколько в нем элементов
            //В этом случае, нужно использовать List
            //Обратите внимание на пространство имен System.Collections.Generic
            //Угловые скобки пока - волшебные слова. 
            //На самом деле это дженерик-тип, но мы поговорим об этом позже.
            List<int> list = new List<int>();

            list.Add(0);
            list.Add(2);
            list.Add(3);
            list.Insert(1, 1);
            list.RemoveAt(0);

            foreach (var e in list)
                Console.WriteLine(e);


        }
    }
}
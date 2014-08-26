using System;
using uLearn;

namespace Slide03
{
	//#video hqoVg1bFwcY
	/*
	[Материалы по лекции](/Courses/BasicProgramming/U05_Collections/_S030_StringToMethod.odp)
	*/
	/*
	## Заметки по лекции
	*/
	[Slide("Сравнение строк и массивов", "{3FFACAA3-2097-4BE9-A9A6-8216DAD73FDA}")]
	public class S030_ArrAndStringDiff
    {
        static void Main()
        {
            string str = "abc";
            //Если раскомментировать эту строку, возникнет ошибка: строки нельзя модифицировать.
            //Не существует никакого способа изменить состояние конкретной строки.
            //str[0] = 'a';
        }
    }
}
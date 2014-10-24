using uLearn;

namespace OOP.Slides.U03_DataStructure
{
	[Slide("Задача. Ассоциативный массив", "{6F904BCF-1B0A-46C8-9384-3C0BD28A50C6}")]
	public class S020_IAssociativeArray
	{
        /*
        Данная задача является обязательной.
        Необходимо реализовать ассоциативный массив, где ключем является строка, а значением - целое число, с помощью хэш-таблицы, самобалансирующегося дерева поиска или списка с пропусками:
        */

        public interface IAssociativeArray
        {
            int Find(string key);
            int Insert(string key, int value);
            int Remove(string key);
        }

        /*
        Операция Insert возвращает предыдущее значение, которое было ассоциировано с ключем key. Операция Remove возвращает значение, которое было ассоциировано с ключем key.
        
        Использовать встроенные в .NET реализации(Dictionary, SortedDictionary и т.д) использовать нельзя.
        */
    }
}
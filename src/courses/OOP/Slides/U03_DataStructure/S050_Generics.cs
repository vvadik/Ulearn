using System;
using uLearn;

namespace OOP.Slides.U03_DataStructure
{
    [Slide("Задача. Обобщенное программирование", "{BB5A40EC-B9FE-48E8-A441-3F368C65DF12}")]
    public class S050_Generics
    {
        /*
        Решая предыдущие задачи(ассоциативный массив, куча) можно было заметить, что для данных структур данных не важно, 
        над какими типами они работают. Ассоциативный массив, в котором ключами и значениями являются строки, ничем не отличается от ассоциативного массива,
        в котором ключами и значениями являются целые числа. Соответственно, не хочется иметь отдельные реализации под каждую пару типов ключ-значение, 
        а иметь одную общую реализацию ассоциативного массива, для которого можно задавать необходимые типы для ключа и значения.
        
        Данную проблему решает техника, которая называется обобщенным программированием(https://en.wikipedia.org/wiki/Generic_programming).
        Эта техника позволяет описать ассоциативный массив следующим образом(здесь TKey - тип ключа, а TValue - тип значения):
        */

        public interface IAssociativeArray<TKey, TValue>
        {
            TValue Find(TKey key);
            TValue Insert(TKey key, TValue value);
            TValue Remove(TKey key);
        }

        public class AssociativeArray<TKey, TValue> : IAssociativeArray<TKey, TValue>
        {
            public TValue Find(TKey key)
            {
                return default(TValue);
            }

            public TValue Insert(TKey key, TValue value)
            {
                return default(TValue);
            }

            public TValue Remove(TKey key)
            {
                return default(TValue);
            }
        }

        public class AssociativeArrayUsage
        {
            public static void Main(string[] args)
            {
                IAssociativeArray<string, int> wordsCount = new AssociativeArray<string, int>();
                wordsCount.Insert("lalala", 1);
                wordsCount.Insert("bububu", 2);
            }
        }
        /*
        В данном примере TKey и TValue называются параметризуемыми типами, IAssociativeArray<TKey, TValue> - обобщенным типом.
        
        При реализации обобщенных типов часто возникает необходимость наложения ограничений на параметризуемые типы.
        Например, при реализации ассоциативного массива с помощью самобалансирующегося дерева поиска, необходимо 
        чтобы экземпляры TKey был упорядочиваемыми. Одна из возможностей сделать в С# тип упорядочиваемым - реализовать в типе интерфейс
        IComparable(http://msdn.microsoft.com/ru-ru/library/system.icomparable.aspx). 
        Значит, необходимо наложить на TKey следующее ограничение: он должен реализовывать интерфейс IComparable<TKey>:
        */

        public interface IOrderedAssociativeArray<TKey, TValue> where TKey : IComparable<TKey>
        {
            TValue Find(TKey key);
            TValue Insert(TKey key, TValue value);
            TValue Remove(TKey key);
        }

        /*
        
        ## Задача
        Данная задача является обязательной.
        Необходимо реализовать обобщенные варианты IAssosiativeArray и IHeap. 
        Не забудьте протестировать ваши реализации.
        */
    }
}
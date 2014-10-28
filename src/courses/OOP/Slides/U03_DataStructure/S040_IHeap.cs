using uLearn;

namespace OOP.Slides.U03_DataStructure
{
    [Slide("Задача. Куча", "{617ADD7C-3093-4B4B-9BDF-E99DA4DB1D11}")]
    public class S040_IHeap
    {
        /*
        Данная задача является обязательной.

        Необходимо реализовать max-кучу (бинарную, биномиальную или Фибоначчиеву кучу), где элементами являются целые числа:
       */

        public interface Heap
        {
            void add(int elemet);
            int findMax();
            int deleteMax();
        }

        /* 
        Не забудьте протестировать вашу реализацию.
        
        Использовать встроенные в Java коллекции — Map, Set, List и производные — нельзя, массивы можно.
        */
    }
}
using uLearn;

namespace OOP.Slides.U03_DataStructure
{
    [Slide("Задача. Куча", "{617ADD7C-3093-4B4B-9BDF-E99DA4DB1D11}")]
    public class S040_IHeap
    {
        /*
        Данная задача является обязательной.
        Необходимо реализовать max-кучу(бинарную, биномиальную и фибоначиеву кучу), где элементами являются целые числа:
       */

        public interface IHeap
        {
            void Add(int elemet);
            int FindMax();
            int DeleteMax();
            IHeap Merge(IHeap other);
        }

        /* 
        Не забывайте протестировать вашу реализацию.
        
        Использовать встроенные в .NET коллекции(Dictionary, Set, List и производные) нельзя, массивы - можно.
        */
    }
}